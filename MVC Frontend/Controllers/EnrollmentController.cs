using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Frontend.Models;
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

        // ══════════════════════════════════════════════════════
        // TRAINEE — My Enrollments
        // ══════════════════════════════════════════════════════

        // Shows the current trainee's enrollment history with optional status filter
        [Authorize(Roles = AppRoles.Trainee)]
        public async Task<IActionResult> MyEnrollments(int? statusId)
        {
            var trainee = await GetCurrentTraineeAsync();
            if (trainee == null)
            {
                TempData["Error"] = "Trainee profile not found.";
                return RedirectToAction("Index", "Home");
            }

            var query = _context.Enrollments
                .Include(e => e.EnrollmentStatus)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Where(e => e.TraineeId == trainee.TraineeId)
                .AsQueryable();

            if (statusId.HasValue)
                query = query.Where(e => e.EnrollmentStatusId == statusId.Value);

            var enrollments = await query
                .OrderByDescending(e => e.EnrollmentDate)
                .ToListAsync();

            var items = enrollments.Select(e => new EnrollmentListItemViewModel
            {
                EnrollmentId    = e.EnrollmentId,
                TraineeId       = e.TraineeId,
                TraineeName     = User.Identity?.Name ?? string.Empty,
                SessionId       = e.SessionId,
                CourseTitle     = e.Session.Course.Title,
                CourseCode      = e.Session.Course.CourseCode,
                SessionDate     = e.Session.SessionDate,
                StatusName      = e.EnrollmentStatus.Status,
                EnrollmentDate  = e.EnrollmentDate,
                DropReason      = e.DropReason
            }).ToList();

            // Populate status filter dropdown
            ViewBag.Statuses = new SelectList(
                await _context.EnrollmentStatuses.ToListAsync(),
                "EnrollmentStatusId", "Status", statusId);
            ViewBag.SelectedStatusId = statusId;

            return View(items);
        }

        // ══════════════════════════════════════════════════════
        // TRAINEE — Enroll in a session
        // ══════════════════════════════════════════════════════

        // Shows session info and an enroll button; validates eligibility before rendering
        [Authorize(Roles = AppRoles.Trainee)]
        public async Task<IActionResult> Enroll(int sessionId)
        {
            var trainee = await GetCurrentTraineeAsync();
            if (trainee == null)
            {
                TempData["Error"] = "Trainee profile not found.";
                return RedirectToAction("Index", "Home");
            }

            var session = await _context.CourseSessions
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .Include(s => s.Classroom)
                .Include(s => s.Status)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null)
                return NotFound();

            // Only Scheduled sessions are open for enrollment
            if (session.Status.Status != "Scheduled")
            {
                TempData["Error"] = "This session is not open for enrollment.";
                return RedirectToAction("Index", "Session");
            }

            if (session.CurrentEnrollment >= session.MaxCapacity)
            {
                TempData["Error"] = "This session is full. No spots available.";
                return RedirectToAction("Index", "Session");
            }

            // Prevent duplicate active enrollments in the same session
            var alreadyEnrolled = await _context.Enrollments.AnyAsync(e =>
                e.SessionId == sessionId &&
                e.TraineeId == trainee.TraineeId &&
                e.EnrollmentStatus.Status != "Dropped");

            if (alreadyEnrolled)
            {
                TempData["Info"] = "You are already enrolled in this session.";
                return RedirectToAction(nameof(MyEnrollments));
            }

            var instructorUser = await _context.Users.FindAsync(session.Instructor.UserId);

            var vm = new EnrollSessionViewModel
            {
                SessionId          = session.SessionId,
                CourseTitle        = session.Course.Title,
                CourseCode         = session.Course.CourseCode,
                CourseDescription  = session.Course.Description,
                DurationHours      = session.Course.DurationHours,
                EnrollmentFee      = session.Course.EnrollmentFee,
                SessionDate        = session.SessionDate,
                StartTime          = session.StartTime,
                EndTime            = session.EndTime,
                InstructorName     = instructorUser?.UserName ?? $"Instructor {session.InstructorId}",
                ClassroomName      = session.Classroom.Name,
                ClassroomLocation  = session.Classroom.Location,
                CurrentEnrollment  = session.CurrentEnrollment,
                MaxCapacity        = session.MaxCapacity
            };

            return View(vm);
        }

        // Processes the enrollment after the trainee confirms
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Trainee)]
        public async Task<IActionResult> EnrollConfirm(int sessionId)
        {
            var trainee = await GetCurrentTraineeAsync();
            if (trainee == null)
            {
                TempData["Error"] = "Trainee profile not found.";
                return RedirectToAction("Index", "Home");
            }

            var session = await _context.CourseSessions
                .Include(s => s.Status)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null)
                return NotFound();

            if (session.Status.Status != "Scheduled")
            {
                TempData["Error"] = "This session is no longer open for enrollment.";
                return RedirectToAction("Index", "Session");
            }

            if (session.CurrentEnrollment >= session.MaxCapacity)
            {
                TempData["Error"] = "Sorry, this session just became full.";
                return RedirectToAction("Index", "Session");
            }

            var alreadyEnrolled = await _context.Enrollments.AnyAsync(e =>
                e.SessionId == sessionId &&
                e.TraineeId == trainee.TraineeId &&
                e.EnrollmentStatus.Status != "Dropped");

            if (alreadyEnrolled)
            {
                TempData["Info"] = "You are already enrolled in this session.";
                return RedirectToAction(nameof(MyEnrollments));
            }

            // New enrollments start with status "Enrolled" and await coordinator confirmation
            var enrolledStatus = await _context.EnrollmentStatuses
                .FirstAsync(s => s.Status == "Enrolled");

            var enrollment = new Enrollment
            {
                SessionId          = sessionId,
                TraineeId          = trainee.TraineeId,
                EnrollmentStatusId = enrolledStatus.EnrollmentStatusId,
                EnrollmentDate     = DateTime.Now,
                StatusChangedAt    = DateTime.Now,
                CreatedAt          = DateTime.Now,
                UpdatedAt          = DateTime.Now
            };

            _context.Enrollments.Add(enrollment);
            session.CurrentEnrollment += 1;
            await _context.SaveChangesAsync();

            TempData["Success"] = "You have successfully enrolled. Awaiting coordinator confirmation.";
            return RedirectToAction(nameof(MyEnrollments));
        }

        // ══════════════════════════════════════════════════════
        // TRAINEE — Cancel enrollment
        // ══════════════════════════════════════════════════════

        // Shows a cancellation confirmation form with an optional drop-reason field
        [Authorize(Roles = AppRoles.Trainee)]
        public async Task<IActionResult> Cancel(int id)
        {
            var trainee = await GetCurrentTraineeAsync();
            if (trainee == null) return Forbid();

            var enrollment = await _context.Enrollments
                .Include(e => e.EnrollmentStatus)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(e => e.EnrollmentId == id && e.TraineeId == trainee.TraineeId);

            if (enrollment == null)
                return NotFound();

            // Cannot cancel enrollments that are already dropped or completed
            if (enrollment.EnrollmentStatus.Status == "Dropped" ||
                enrollment.EnrollmentStatus.Status == "Completed")
            {
                TempData["Error"] = "This enrollment cannot be cancelled.";
                return RedirectToAction(nameof(MyEnrollments));
            }

            var vm = new CancelEnrollmentViewModel
            {
                EnrollmentId = enrollment.EnrollmentId,
                CourseTitle  = enrollment.Session.Course.Title,
                CourseCode   = enrollment.Session.Course.CourseCode,
                SessionDate  = enrollment.Session.SessionDate,
                StatusName   = enrollment.EnrollmentStatus.Status
            };

            return View(vm);
        }

        // Processes the cancellation and marks the enrollment as "Dropped"
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Trainee)]
        public async Task<IActionResult> CancelConfirm(CancelEnrollmentViewModel vm)
        {
            var trainee = await GetCurrentTraineeAsync();
            if (trainee == null) return Forbid();

            var enrollment = await _context.Enrollments
                .Include(e => e.EnrollmentStatus)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(e => e.EnrollmentId == vm.EnrollmentId && e.TraineeId == trainee.TraineeId);

            if (enrollment == null)
                return NotFound();

            if (enrollment.EnrollmentStatus.Status == "Dropped" ||
                enrollment.EnrollmentStatus.Status == "Completed")
            {
                TempData["Error"] = "This enrollment cannot be cancelled.";
                return RedirectToAction(nameof(MyEnrollments));
            }

            if (!ModelState.IsValid)
            {
                vm.CourseTitle = enrollment.Session.Course.Title;
                vm.CourseCode  = enrollment.Session.Course.CourseCode;
                vm.SessionDate = enrollment.Session.SessionDate;
                vm.StatusName  = enrollment.EnrollmentStatus.Status;
                return View("Cancel", vm);
            }

            var droppedStatus = await _context.EnrollmentStatuses
                .FirstAsync(s => s.Status == "Dropped");

            enrollment.EnrollmentStatusId = droppedStatus.EnrollmentStatusId;
            enrollment.DropReason         = vm.DropReason;
            enrollment.StatusChangedAt    = DateTime.Now;
            enrollment.UpdatedAt          = DateTime.Now;

            // Decrement the session seat counter so the spot opens up again
            enrollment.Session.CurrentEnrollment =
                Math.Max(0, enrollment.Session.CurrentEnrollment - 1);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Your enrollment has been cancelled.";
            return RedirectToAction(nameof(MyEnrollments));
        }

        // ══════════════════════════════════════════════════════
        // SHARED — Enrollment details
        // ══════════════════════════════════════════════════════

        // Trainee can only see their own; coordinator can see any enrollment
        public async Task<IActionResult> Details(int id)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.EnrollmentStatus)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Include(e => e.Session.Instructor)
                .Include(e => e.Session.Classroom)
                .Include(e => e.Trainee)
                .Include(e => e.Assessments)
                    .ThenInclude(a => a.Instructor)
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (enrollment == null)
                return NotFound();

            // Trainees may only view their own enrollment
            if (User.IsInRole(AppRoles.Trainee))
            {
                var trainee = await GetCurrentTraineeAsync();
                if (trainee == null || enrollment.TraineeId != trainee.TraineeId)
                    return Forbid();
            }

            var traineeUser    = await _context.Users.FindAsync(enrollment.Trainee.UserId);
            var instructorUser = await _context.Users.FindAsync(enrollment.Session.Instructor.UserId);

            // Resolve instructor name for each assessment
            var assessmentItems = new List<AssessmentSummaryViewModel>();
            foreach (var a in enrollment.Assessments)
            {
                var aInstructor = await _context.Users.FindAsync(a.Instructor.UserId);
                assessmentItems.Add(new AssessmentSummaryViewModel
                {
                    AssessmentId   = a.AssessmentId,
                    InstructorName = aInstructor?.UserName ?? $"Instructor {a.InstructorId}",
                    Result         = a.Result,
                    Remarks        = a.Remarks,
                    AssessmentDate = a.AssessmentDate
                });
            }

            var vm = new EnrollmentDetailsViewModel
            {
                EnrollmentId      = enrollment.EnrollmentId,
                TraineeId         = enrollment.TraineeId,
                TraineeName       = traineeUser?.UserName ?? $"Trainee {enrollment.TraineeId}",
                TraineeEmail      = traineeUser?.Email ?? string.Empty,
                SessionId         = enrollment.SessionId,
                CourseId          = enrollment.Session.CourseId,
                CourseTitle       = enrollment.Session.Course.Title,
                CourseCode        = enrollment.Session.Course.CourseCode,
                SessionDate       = enrollment.Session.SessionDate,
                StartTime         = enrollment.Session.StartTime,
                EndTime           = enrollment.Session.EndTime,
                InstructorName    = instructorUser?.UserName ?? $"Instructor {enrollment.Session.InstructorId}",
                ClassroomName     = enrollment.Session.Classroom.Name,
                EnrollmentStatusId = enrollment.EnrollmentStatusId,
                StatusName        = enrollment.EnrollmentStatus.Status,
                EnrollmentDate    = enrollment.EnrollmentDate,
                StatusChangedAt   = enrollment.StatusChangedAt,
                DropReason        = enrollment.DropReason,
                Assessments       = assessmentItems
            };

            // Populate statuses for the coordinator's inline status-update form
            ViewBag.Statuses = new SelectList(
                await _context.EnrollmentStatuses.ToListAsync(),
                "EnrollmentStatusId", "Status", enrollment.EnrollmentStatusId);

            return View(vm);
        }

        // ══════════════════════════════════════════════════════
        // COORDINATOR — All Enrollments
        // ══════════════════════════════════════════════════════

        // Full enrollment list with filters: session, trainee, status, date range
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> AllEnrollments(
            int? sessionId, int? traineeId, int? statusId,
            string? dateFrom, string? dateTo)
        {
            var query = _context.Enrollments
                .Include(e => e.EnrollmentStatus)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Include(e => e.Trainee)
                .AsQueryable();

            if (sessionId.HasValue)
                query = query.Where(e => e.SessionId == sessionId.Value);
            if (traineeId.HasValue)
                query = query.Where(e => e.TraineeId == traineeId.Value);
            if (statusId.HasValue)
                query = query.Where(e => e.EnrollmentStatusId == statusId.Value);
            if (!string.IsNullOrWhiteSpace(dateFrom) && DateTime.TryParse(dateFrom, out var from))
                query = query.Where(e => e.EnrollmentDate >= from);
            if (!string.IsNullOrWhiteSpace(dateTo) && DateTime.TryParse(dateTo, out var to))
                query = query.Where(e => e.EnrollmentDate <= to);

            var enrollments = await query
                .OrderByDescending(e => e.EnrollmentDate)
                .ToListAsync();

            // Resolve trainee display names from Identity (stored separately from Trainee table)
            var traineeUserIds = enrollments.Select(e => e.Trainee.UserId).Distinct().ToList();
            var traineeUsers   = await _context.Users
                .Where(u => traineeUserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.UserName ?? u.Email ?? u.Id);

            var items = enrollments.Select(e => new EnrollmentListItemViewModel
            {
                EnrollmentId   = e.EnrollmentId,
                TraineeId      = e.TraineeId,
                TraineeName    = traineeUsers.TryGetValue(e.Trainee.UserId, out var n) ? n : $"Trainee {e.TraineeId}",
                SessionId      = e.SessionId,
                CourseTitle    = e.Session.Course.Title,
                CourseCode     = e.Session.Course.CourseCode,
                SessionDate    = e.Session.SessionDate,
                StatusName     = e.EnrollmentStatus.Status,
                EnrollmentDate = e.EnrollmentDate,
                DropReason     = e.DropReason
            }).ToList();

            // Build trainee dropdown options for the filter bar
            var allTrainees    = await _context.Trainees.ToListAsync();
            var allTraineeUids = allTrainees.Select(t => t.UserId).ToList();
            var allTraineeUsrs = await _context.Users
                .Where(u => allTraineeUids.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.UserName ?? u.Email ?? u.Id);

            var vm = new AllEnrollmentsViewModel
            {
                Enrollments       = items,
                FilterSessionId   = sessionId,
                FilterTraineeId   = traineeId,
                FilterStatusId    = statusId,
                FilterDateFrom    = dateFrom,
                FilterDateTo      = dateTo,
                Sessions          = await _context.CourseSessions
                    .Include(s => s.Course)
                    .OrderBy(s => s.SessionDate)
                    .Select(s => new SelectListItem
                    {
                        Value = s.SessionId.ToString(),
                        Text  = $"{s.Course.CourseCode} — {s.SessionDate:dd MMM yyyy}"
                    })
                    .ToListAsync(),
                Trainees = allTrainees
                    .Select(t => new SelectListItem
                    {
                        Value = t.TraineeId.ToString(),
                        Text  = allTraineeUsrs.TryGetValue(t.UserId, out var tn)
                                ? tn : $"Trainee {t.TraineeId}"
                    })
                    .OrderBy(x => x.Text)
                    .ToList(),
                Statuses = await _context.EnrollmentStatuses
                    .Select(s => new SelectListItem
                    {
                        Value = s.EnrollmentStatusId.ToString(),
                        Text  = s.Status
                    })
                    .ToListAsync()
            };

            return View(vm);
        }

        // ══════════════════════════════════════════════════════
        // COORDINATOR — Update enrollment status
        // ══════════════════════════════════════════════════════

        // Allows the coordinator to move an enrollment to any status (approve/reject/complete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> UpdateStatus(UpdateEnrollmentStatusViewModel vm)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Session)
                .FirstOrDefaultAsync(e => e.EnrollmentId == vm.EnrollmentId);

            if (enrollment == null)
                return NotFound();

            var newStatus = await _context.EnrollmentStatuses
                .FindAsync(vm.NewStatusId);
            if (newStatus == null)
            {
                TempData["Error"] = "Invalid status selected.";
                return RedirectToAction(nameof(Details), new { id = vm.EnrollmentId });
            }

            var oldStatus = await _context.EnrollmentStatuses
                .FindAsync(enrollment.EnrollmentStatusId);

            // Adjust the session seat counter when moving to/from "Dropped"
            if (newStatus.Status == "Dropped" && oldStatus?.Status != "Dropped")
                enrollment.Session.CurrentEnrollment = Math.Max(0, enrollment.Session.CurrentEnrollment - 1);
            else if (oldStatus?.Status == "Dropped" && newStatus.Status != "Dropped")
                enrollment.Session.CurrentEnrollment += 1;

            enrollment.EnrollmentStatusId = vm.NewStatusId;
            enrollment.DropReason         = vm.DropReason;
            enrollment.StatusChangedAt    = DateTime.Now;
            enrollment.UpdatedAt          = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Enrollment status updated to \"{newStatus.Status}\".";
            return RedirectToAction(nameof(Details), new { id = vm.EnrollmentId });
        }

        // ══════════════════════════════════════════════════════
        // COORDINATOR — Manual enrollment
        // ══════════════════════════════════════════════════════

        // Shows a form where the coordinator can enroll any trainee into any open session
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> ManualEnroll()
        {
            var vm = await BuildManualEnrollDropdownsAsync(new ManualEnrollViewModel());
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> ManualEnroll(ManualEnrollViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(await BuildManualEnrollDropdownsAsync(vm));

            var session = await _context.CourseSessions
                .Include(s => s.Status)
                .FirstOrDefaultAsync(s => s.SessionId == vm.SessionId);

            if (session == null || session.Status.Status != "Scheduled")
            {
                ModelState.AddModelError(string.Empty, "The selected session is not open for enrollment.");
                return View(await BuildManualEnrollDropdownsAsync(vm));
            }

            if (session.CurrentEnrollment >= session.MaxCapacity)
            {
                ModelState.AddModelError(string.Empty, "The selected session is at full capacity.");
                return View(await BuildManualEnrollDropdownsAsync(vm));
            }

            var alreadyEnrolled = await _context.Enrollments.AnyAsync(e =>
                e.SessionId == vm.SessionId &&
                e.TraineeId == vm.TraineeId &&
                e.EnrollmentStatus.Status != "Dropped");

            if (alreadyEnrolled)
            {
                ModelState.AddModelError(string.Empty, "This trainee is already enrolled in the selected session.");
                return View(await BuildManualEnrollDropdownsAsync(vm));
            }

            // Coordinator-created enrollments go straight to "Confirmed"
            var confirmedStatus = await _context.EnrollmentStatuses
                .FirstAsync(s => s.Status == "Confirmed");

            var enrollment = new Enrollment
            {
                SessionId          = vm.SessionId,
                TraineeId          = vm.TraineeId,
                EnrollmentStatusId = confirmedStatus.EnrollmentStatusId,
                EnrollmentDate     = DateTime.Now,
                StatusChangedAt    = DateTime.Now,
                CreatedAt          = DateTime.Now,
                UpdatedAt          = DateTime.Now
            };

            _context.Enrollments.Add(enrollment);
            session.CurrentEnrollment += 1;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Trainee enrolled successfully.";
            return RedirectToAction(nameof(AllEnrollments));
        }

        // ══════════════════════════════════════════════════════
        // INSTRUCTOR — Session roster
        // ══════════════════════════════════════════════════════

        // Lists all sessions the instructor teaches along with each session's enrolled trainees
        [Authorize(Roles = AppRoles.Instructor)]
        public async Task<IActionResult> MySessions()
        {
            var userId     = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == userId);

            if (instructor == null)
            {
                TempData["Error"] = "Instructor profile not found.";
                return RedirectToAction("Index", "Home");
            }

            var sessions = await _context.CourseSessions
                .Include(s => s.Course)
                .Include(s => s.Status)
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.EnrollmentStatus)
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Trainee)
                .Where(s => s.InstructorId == instructor.InstructorId)
                .OrderByDescending(s => s.SessionDate)
                .ToListAsync();

            // Collect all trainee userId values so we can batch-fetch display names and emails
            var allTraineeUserIds = sessions
                .SelectMany(s => s.Enrollments.Select(e => e.Trainee.UserId))
                .Distinct()
                .ToList();

            var traineeUserRecords = await _context.Users
                .Where(u => allTraineeUserIds.Contains(u.Id))
                .ToListAsync();

            // Two dictionaries: one for names, one for emails — avoids N+1 queries later
            var traineeUsers  = traineeUserRecords.ToDictionary(u => u.Id, u => u.UserName ?? u.Email ?? u.Id);
            var traineeEmails = traineeUserRecords.ToDictionary(u => u.Id, u => u.Email ?? string.Empty);

            // Also need to know which enrollments already have assessments
            var allEnrollmentIds = sessions
                .SelectMany(s => s.Enrollments.Select(e => e.EnrollmentId))
                .ToList();

            var assessedEnrollmentIds = await _context.Assessments
                .Where(a => allEnrollmentIds.Contains(a.EnrollmentId))
                .Select(a => a.EnrollmentId)
                .ToHashSetAsync();

            var rosters = sessions.Select(s => new InstructorSessionRosterViewModel
            {
                SessionId         = s.SessionId,
                CourseTitle       = s.Course.Title,
                CourseCode        = s.Course.CourseCode,
                SessionDate       = s.SessionDate,
                StartTime         = s.StartTime,
                EndTime           = s.EndTime,
                StatusName        = s.Status.Status,
                CurrentEnrollment = s.CurrentEnrollment,
                MaxCapacity       = s.MaxCapacity,
                Trainees          = s.Enrollments
                    .Where(e => e.EnrollmentStatus.Status != "Dropped")
                    .Select(e => new TraineeRosterItemViewModel
                    {
                        EnrollmentId     = e.EnrollmentId,
                        TraineeId        = e.TraineeId,
                        TraineeName      = traineeUsers.TryGetValue(e.Trainee.UserId, out var tn) ? tn : $"Trainee {e.TraineeId}",
                        TraineeEmail     = traineeEmails.TryGetValue(e.Trainee.UserId, out var te) ? te : string.Empty,
                        EnrollmentStatus = e.EnrollmentStatus.Status,
                        EnrollmentDate   = e.EnrollmentDate,
                        HasAssessment    = assessedEnrollmentIds.Contains(e.EnrollmentId)
                    })
                    .OrderBy(t => t.TraineeName)
                    .ToList()
            }).ToList();

            return View(rosters);
        }

        // ══════════════════════════════════════════════════════
        // Helpers
        // ══════════════════════════════════════════════════════

        // Looks up the Trainee record that matches the currently authenticated user
        private async Task<Trainee?> GetCurrentTraineeAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.Trainees.FirstOrDefaultAsync(t => t.UserId == userId);
        }

        // Fills the session and trainee dropdowns for the ManualEnroll form
        private async Task<ManualEnrollViewModel> BuildManualEnrollDropdownsAsync(ManualEnrollViewModel vm)
        {
            var openSessions = await _context.CourseSessions
                .Include(s => s.Course)
                .Include(s => s.Status)
                .Where(s => s.Status.Status == "Scheduled" && s.CurrentEnrollment < s.MaxCapacity)
                .OrderBy(s => s.SessionDate)
                .ToListAsync();

            vm.Sessions = openSessions
                .Select(s => new SelectListItem
                {
                    Value = s.SessionId.ToString(),
                    Text  = $"{s.Course.CourseCode} — {s.Course.Title} ({s.SessionDate:dd MMM yyyy}) [{s.CurrentEnrollment}/{s.MaxCapacity}]"
                })
                .ToList();

            var allTrainees  = await _context.Trainees.ToListAsync();
            var traineeUids  = allTrainees.Select(t => t.UserId).ToList();
            var traineeUsers = await _context.Users
                .Where(u => traineeUids.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.UserName ?? u.Email ?? u.Id);

            vm.Trainees = allTrainees
                .Select(t => new SelectListItem
                {
                    Value = t.TraineeId.ToString(),
                    Text  = traineeUsers.TryGetValue(t.UserId, out var tn) ? tn : $"Trainee {t.TraineeId}"
                })
                .OrderBy(x => x.Text)
                .ToList();

            return vm;
        }
    }
}
