using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Frontend.Models;
using System.Security.Claims;
using Web_API.Models;

namespace MVC_Frontend.Controllers
{
    [Authorize]
    public class EnrollmentController : Controller
    {
        private readonly TrainingInstituteDBContext _context;

        public EnrollmentController(TrainingInstituteDBContext context)
        {
            _context = context;
        }

        // ── ADV-41: Display enrolled sessions for trainee ─────────────────────
        [Authorize(Roles = "Trainee")]
        public async Task<IActionResult> MyEnrollments()
        {
            var trainee = await GetCurrentTraineeAsync();
            if (trainee == null) return RedirectToLogin();

            var enrollments = await _context.Enrollments
                .Include(e => e.EnrollmentStatus)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                        .ThenInclude(c => c.SubjectArea)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Instructor)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Status)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Classroom)
                .Where(e => e.TraineeId == trainee.TraineeId)
                .OrderByDescending(e => e.Session.SessionDate)
                .ToListAsync();

            var waitlists = await _context.Waitlists
                .Include(w => w.Session)
                    .ThenInclude(s => s.Course)
                .Where(w => w.TraineeId == trainee.TraineeId && w.Status == "Waiting")
                .OrderBy(w => w.Position)
                .ToListAsync();

            ViewBag.Waitlists = waitlists;

            if (TempData["Success"] != null)
                ViewBag.Success = TempData["Success"];
            if (TempData["Error"] != null)
                ViewBag.Error = TempData["Error"];

            return View(enrollments);
        }

        // ── ADV-36: Enrollment confirmation page ──────────────────────────────
        [Authorize(Roles = "Trainee")]
        public async Task<IActionResult> Enroll(int sessionId)
        {
            var trainee = await GetCurrentTraineeAsync();
            if (trainee == null) return RedirectToLogin();

            var session = await _context.CourseSessions
                .Include(s => s.Course)
                    .ThenInclude(c => c.PrerequisiteCourse)
                .Include(s => s.Course)
                    .ThenInclude(c => c.Category)
                .Include(s => s.Course)
                    .ThenInclude(c => c.SubjectArea)
                .Include(s => s.Instructor)
                .Include(s => s.Classroom)
                .Include(s => s.Status)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null)
                return NotFound();

            var vm = new EnrollConfirmViewModel { Session = session };

            // ADV-37: prerequisite gate
            if (session.Course.PrerequisiteCourseId.HasValue)
            {
                vm.HasPrerequisite = true;
                vm.PrerequisiteCourseName = session.Course.PrerequisiteCourse?.Title;
                vm.PrerequisiteCourseCode = session.Course.PrerequisiteCourse?.CourseCode;

                vm.HasPassedPrerequisite = await _context.TraineeCourseCompletions
                    .AnyAsync(tc =>
                        tc.TraineeId == trainee.TraineeId &&
                        tc.CourseId == session.Course.PrerequisiteCourseId.Value &&
                        tc.Result == "Pass");
            }
            else
            {
                vm.HasPassedPrerequisite = true;
            }

            // Duplicate and waitlist checks
            vm.IsAlreadyEnrolled = await _context.Enrollments
                .Include(e => e.EnrollmentStatus)
                .AnyAsync(e =>
                    e.SessionId == sessionId &&
                    e.TraineeId == trainee.TraineeId &&
                    e.EnrollmentStatus.Status != "Dropped");

            vm.IsOnWaitlist = await _context.Waitlists
                .AnyAsync(w =>
                    w.SessionId == sessionId &&
                    w.TraineeId == trainee.TraineeId &&
                    w.Status == "Waiting");

            return View(vm);
        }

        // ── ADV-36/38/39: Perform enrollment ─────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Trainee")]
        public async Task<IActionResult> Enroll(int sessionId, string action)
        {
            var trainee = await GetCurrentTraineeAsync();
            if (trainee == null) return RedirectToLogin();

            var session = await _context.CourseSessions
                .Include(s => s.Course)
                .Include(s => s.Status)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null)
                return NotFound();

            // Handle waitlist join shortcut
            if (action == "waitlist")
                return await JoinWaitlistInternal(trainee, session);

            // Session must be Scheduled
            if (session.Status.Status != "Scheduled")
            {
                TempData["Error"] = "Enrollment is only available for scheduled sessions.";
                return RedirectToAction(nameof(Enroll), new { sessionId });
            }

            // Duplicate check
            var alreadyEnrolled = await _context.Enrollments
                .Include(e => e.EnrollmentStatus)
                .AnyAsync(e =>
                    e.SessionId == sessionId &&
                    e.TraineeId == trainee.TraineeId &&
                    e.EnrollmentStatus.Status != "Dropped");

            if (alreadyEnrolled)
            {
                TempData["Error"] = "You are already enrolled in this session.";
                return RedirectToAction(nameof(Enroll), new { sessionId });
            }

            // ADV-37: prerequisite validation
            if (session.Course.PrerequisiteCourseId.HasValue)
            {
                var passed = await _context.TraineeCourseCompletions
                    .AnyAsync(tc =>
                        tc.TraineeId == trainee.TraineeId &&
                        tc.CourseId == session.Course.PrerequisiteCourseId.Value &&
                        tc.Result == "Pass");

                if (!passed)
                {
                    var prereq = await _context.Courses.FindAsync(session.Course.PrerequisiteCourseId.Value);
                    TempData["Error"] = $"You must complete \"{prereq?.Title}\" before enrolling in this course.";
                    return RedirectToAction(nameof(Enroll), new { sessionId });
                }
            }

            // ADV-38: capacity check
            if (session.CurrentEnrollment >= session.MaxCapacity)
            {
                TempData["Error"] = "This session is full.";
                TempData["SessionFull"] = "true";
                return RedirectToAction(nameof(Enroll), new { sessionId });
            }

            // Create enrollment record
            var enrolledStatus = await _context.EnrollmentStatuses.FirstAsync(s => s.Status == "Enrolled");

            _context.Enrollments.Add(new Enrollment
            {
                SessionId = sessionId,
                TraineeId = trainee.TraineeId,
                EnrollmentStatusId = enrolledStatus.EnrollmentStatusId,
                EnrollmentDate = DateTime.Now,
                StatusChangedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });

            // ADV-39: decrement available seats
            session.CurrentEnrollment++;
            session.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"You are now enrolled in {session.Course.Title}.";
            return RedirectToAction(nameof(MyEnrollments));
        }

        // ── ADV-40: Enrollment cancellation (trainee view) ────────────────────
        [Authorize(Roles = "Trainee")]
        public async Task<IActionResult> Drop(int enrollmentId)
        {
            var trainee = await GetCurrentTraineeAsync();
            if (trainee == null) return RedirectToLogin();

            var enrollment = await _context.Enrollments
                .Include(e => e.EnrollmentStatus)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId && e.TraineeId == trainee.TraineeId);

            if (enrollment == null)
                return NotFound();

            if (enrollment.EnrollmentStatus.Status is "Dropped" or "Completed")
            {
                TempData["Error"] = "This enrollment cannot be dropped.";
                return RedirectToAction(nameof(MyEnrollments));
            }

            return View(new DropViewModel
            {
                EnrollmentId = enrollmentId,
                CourseTitle = enrollment.Session.Course.Title,
                SessionDate = enrollment.Session.SessionDate,
                CurrentStatus = enrollment.EnrollmentStatus.Status
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Trainee")]
        public async Task<IActionResult> Drop(DropViewModel vm)
        {
            var trainee = await GetCurrentTraineeAsync();
            if (trainee == null) return RedirectToLogin();

            var enrollment = await _context.Enrollments
                .Include(e => e.EnrollmentStatus)
                .Include(e => e.Session)
                .FirstOrDefaultAsync(e => e.EnrollmentId == vm.EnrollmentId && e.TraineeId == trainee.TraineeId);

            if (enrollment == null)
                return NotFound();

            if (enrollment.EnrollmentStatus.Status is "Dropped" or "Completed")
            {
                TempData["Error"] = "This enrollment cannot be dropped.";
                return RedirectToAction(nameof(MyEnrollments));
            }

            var droppedStatus = await _context.EnrollmentStatuses.FirstAsync(s => s.Status == "Dropped");
            enrollment.EnrollmentStatusId = droppedStatus.EnrollmentStatusId;
            enrollment.DropReason = vm.DropReason?.Trim();
            enrollment.StatusChangedAt = DateTime.Now;
            enrollment.UpdatedAt = DateTime.Now;

            // Release the seat
            var session = enrollment.Session;
            session.CurrentEnrollment--;
            session.UpdatedAt = DateTime.Now;

            // Auto-promote first waiting trainee from waitlist
            await PromoteFromWaitlistAsync(session);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Your enrollment has been dropped.";
            return RedirectToAction(nameof(MyEnrollments));
        }

        // ── ADV-42: Session enrollment management (Coordinator / Instructor) ──
        [Authorize(Roles = "Training Coordinator,Instructor")]
        public async Task<IActionResult> SessionEnrollments(int sessionId)
        {
            var session = await _context.CourseSessions
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .Include(s => s.Classroom)
                .Include(s => s.Status)
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.EnrollmentStatus)
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Trainee)
                .Include(s => s.Waitlists.Where(w => w.Status == "Waiting"))
                    .ThenInclude(w => w.Trainee)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null)
                return NotFound();

            if (TempData["Success"] != null)
                ViewBag.Success = TempData["Success"];
            if (TempData["Error"] != null)
                ViewBag.Error = TempData["Error"];

            ViewBag.AllStatuses = await _context.EnrollmentStatuses.ToListAsync();
            return View(session);
        }

        // ── ADV-42: Advance enrollment status (Coordinator only) ──────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Training Coordinator")]
        public async Task<IActionResult> UpdateStatus(int enrollmentId, string newStatus)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.EnrollmentStatus)
                .Include(e => e.Session)
                .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);

            if (enrollment == null)
                return NotFound();

            // Enforce valid state machine transitions
            var allowed = enrollment.EnrollmentStatus.Status switch
            {
                "Enrolled"  => new[] { "Confirmed", "Dropped" },
                "Confirmed" => new[] { "Attending", "Dropped" },
                "Attending" => new[] { "Completed", "Dropped" },
                _           => Array.Empty<string>()
            };

            if (!allowed.Contains(newStatus))
            {
                TempData["Error"] = $"Cannot transition from \"{enrollment.EnrollmentStatus.Status}\" to \"{newStatus}\".";
                return RedirectToAction(nameof(SessionEnrollments), new { sessionId = enrollment.SessionId });
            }

            var targetStatus = await _context.EnrollmentStatuses.FirstOrDefaultAsync(s => s.Status == newStatus);
            if (targetStatus == null)
                return BadRequest();

            enrollment.EnrollmentStatusId = targetStatus.EnrollmentStatusId;
            enrollment.StatusChangedAt = DateTime.Now;
            enrollment.UpdatedAt = DateTime.Now;

            if (newStatus == "Dropped")
            {
                var session = enrollment.Session;
                session.CurrentEnrollment--;
                session.UpdatedAt = DateTime.Now;
                await PromoteFromWaitlistAsync(session);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Status updated to \"{newStatus}\".";
            return RedirectToAction(nameof(SessionEnrollments), new { sessionId = enrollment.SessionId });
        }

        // ── ADV-42b: Session list for Instructor ──────────────────────────────
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> MySessions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Account");

            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == userId);
            if (instructor == null) return NotFound();

            var sessions = await _context.CourseSessions
                .Include(s => s.Course)
                    .ThenInclude(c => c.SubjectArea)
                .Include(s => s.Classroom)
                .Include(s => s.Status)
                .Where(s => s.InstructorId == instructor.InstructorId)
                .OrderByDescending(s => s.SessionDate)
                .ToListAsync();

            return View(sessions);
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private async Task<IActionResult> JoinWaitlistInternal(Trainee trainee, CourseSession session)
        {
            var alreadyWaiting = await _context.Waitlists.AnyAsync(w =>
                w.SessionId == session.SessionId &&
                w.TraineeId == trainee.TraineeId &&
                w.Status == "Waiting");

            if (alreadyWaiting)
            {
                TempData["Error"] = "You are already on the waitlist for this session.";
                return RedirectToAction(nameof(Enroll), new { sessionId = session.SessionId });
            }

            var position = await _context.Waitlists
                .CountAsync(w => w.SessionId == session.SessionId && w.Status == "Waiting") + 1;

            var waitingStatus = await _context.WaitlistStatuses.FirstAsync(s => s.Status == "Waiting");

            _context.Waitlists.Add(new Waitlist
            {
                SessionId = session.SessionId,
                TraineeId = trainee.TraineeId,
                StatusId = waitingStatus.StatusId,
                Status = "Waiting",
                Position = position,
                AddedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = $"You have been added to the waitlist at position {position}.";
            return RedirectToAction(nameof(MyEnrollments));
        }

        private async Task PromoteFromWaitlistAsync(CourseSession session)
        {
            var next = await _context.Waitlists
                .Where(w => w.SessionId == session.SessionId && w.Status == "Waiting")
                .OrderBy(w => w.Position)
                .FirstOrDefaultAsync();

            if (next == null) return;

            var enrolledStatus = await _context.EnrollmentStatuses.FirstAsync(s => s.Status == "Enrolled");
            var expiredStatus = await _context.WaitlistStatuses.FirstAsync(s => s.Status == "Expired");

            _context.Enrollments.Add(new Enrollment
            {
                SessionId = session.SessionId,
                TraineeId = next.TraineeId,
                EnrollmentStatusId = enrolledStatus.EnrollmentStatusId,
                EnrollmentDate = DateTime.Now,
                StatusChangedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });

            // Consume the waitlist slot and restore the seat
            next.StatusId = expiredStatus.StatusId;
            next.Status = "Promoted";
            session.CurrentEnrollment++;
        }

        private async Task<Trainee?> GetCurrentTraineeAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return null;
            return await _context.Trainees.FirstOrDefaultAsync(t => t.UserId == userId);
        }

        private IActionResult RedirectToLogin() =>
            RedirectToAction("Login", "Account", new { returnUrl = Request.Path });
    }
}
