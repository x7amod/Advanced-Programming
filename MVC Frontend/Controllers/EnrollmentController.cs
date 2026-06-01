using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MVC_Frontend.Helpers;
using MVC_Frontend.Hubs;
using MVC_Frontend.Models;
using Web_API.Models;

namespace MVC_Frontend.Controllers;

public class EnrollmentController : Controller
{
    private readonly TrainingInstituteDBContext _context;
    private readonly IHubContext<EnrollmentHub> _hubContext;

    public EnrollmentController(TrainingInstituteDBContext context, IHubContext<EnrollmentHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    // ── 1. Browse Available Sessions ─────────────────────────────────────────
    // Visible to everyone; action buttons only shown to authenticated trainees.
    [AllowAnonymous]
    public async Task<IActionResult> Browse(int? subjectAreaId, int? categoryId, string? dateFrom, string? dateTo)
    {
        var today = DateTime.Today;

        var scheduledStatus = await _context.CourseSessionStatuses
            .FirstOrDefaultAsync(s => s.Status == "Scheduled");

        var query = _context.CourseSessions
            .Include(s => s.Course).ThenInclude(c => c.SubjectArea)
            .Include(s => s.Course).ThenInclude(c => c.Category)
            .Include(s => s.Instructor)
            .Include(s => s.Classroom)
            .Include(s => s.Status)
            .Where(s => s.SessionDate >= today);

        if (scheduledStatus != null)
            query = query.Where(s => s.StatusId == scheduledStatus.StatusId);

        if (subjectAreaId.HasValue)
            query = query.Where(s => s.Course.SubjectAreaId == subjectAreaId.Value);
        if (categoryId.HasValue)
            query = query.Where(s => s.Course.CategoryId == categoryId.Value);
        if (!string.IsNullOrWhiteSpace(dateFrom) && DateTime.TryParse(dateFrom, out var from))
            query = query.Where(s => s.SessionDate >= from);
        if (!string.IsNullOrWhiteSpace(dateTo) && DateTime.TryParse(dateTo, out var to))
            query = query.Where(s => s.SessionDate <= to);

        var sessions = await query.OrderBy(s => s.SessionDate).ThenBy(s => s.StartTime).ToListAsync();

        // Resolve instructor display names — pull all users to memory to avoid CTE
        var instructorIds = sessions.Select(s => s.InstructorId).Distinct().ToList();
        var instructors = await _context.Instructors.ToListAsync();
        var allUsers = await _context.Users.ToDictionaryAsync(u => u.Id, u => u.UserName ?? u.Id);
        var instructorNames = instructors
            .Where(i => instructorIds.Contains(i.InstructorId))
            .ToDictionary(
                i => i.InstructorId,
                i => allUsers.TryGetValue(i.UserId, out var n) ? n : $"Instructor {i.InstructorId}");

        // Current trainee's active enrollments and waitlist entries
        var enrolledSessionIds = new HashSet<int>();
        var waitlistedSessionIds = new HashSet<int>();
        if (User.IsInRole(AppRoles.Trainee))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.UserId == userId);
            if (trainee != null)
            {
                var droppedStatus = await _context.EnrollmentStatuses
                    .FirstOrDefaultAsync(s => s.Status == "Dropped");
                var droppedId = droppedStatus?.EnrollmentStatusId ?? 0;

                var active = await _context.Enrollments
                    .Where(e => e.TraineeId == trainee.TraineeId && e.EnrollmentStatusId != droppedId)
                    .Select(e => e.SessionId).ToListAsync();
                enrolledSessionIds = active.ToHashSet();

                var waiting = await _context.Waitlists
                    .Where(w => w.TraineeId == trainee.TraineeId && w.Status != "Expired")
                    .Select(w => w.SessionId).ToListAsync();
                waitlistedSessionIds = waiting.ToHashSet();
            }
        }

        var vm = new BrowseSessionsViewModel
        {
            SubjectAreaId = subjectAreaId,
            CategoryId = categoryId,
            DateFrom = dateFrom,
            DateTo = dateTo,
            SubjectAreas = await _context.SubjectAreas.OrderBy(s => s.Name)
                .Select(s => new SelectListItem { Value = s.SubjectAreaId.ToString(), Text = s.Name })
                .ToListAsync(),
            Categories = await _context.Categories.OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.Name })
                .ToListAsync(),
            Sessions = sessions.Select(s => new SessionCardViewModel
            {
                SessionId = s.SessionId,
                CourseTitle = s.Course.Title,
                InstructorName = instructorNames.TryGetValue(s.InstructorId, out var n) ? n : $"Instructor {s.InstructorId}",
                ClassroomName = s.Classroom.Name,
                SessionDate = s.SessionDate,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                RemainingSpots = s.MaxCapacity - s.CurrentEnrollment,
                MaxCapacity = s.MaxCapacity,
                EnrollmentFee = s.Course.EnrollmentFee,
                IsFull = s.CurrentEnrollment >= s.MaxCapacity,
                IsAlreadyEnrolled = enrolledSessionIds.Contains(s.SessionId),
                IsWaitlisted = waitlistedSessionIds.Contains(s.SessionId)
            }).ToList()
        };

        return View(vm);
    }

    // ── 2. Enroll in Session ─────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.Trainee)]
    public async Task<IActionResult> Enroll(int sessionId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.UserId == userId);
        if (trainee == null)
        {
            TempData["Error"] = "Trainee profile not found. Please contact the administrator.";
            return RedirectToAction(nameof(Browse));
        }

        // Re-query session from DB — never trust client state
        var session = await _context.CourseSessions
            .Include(s => s.Course).ThenInclude(c => c.PrerequisiteCourse)
            .Include(s => s.Status)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session == null) return NotFound();

        // d) Session status check
        if (session.Status.Status != "Scheduled")
        {
            TempData["Error"] = "This session is no longer accepting enrollments.";
            return RedirectToAction(nameof(Browse));
        }

        // a) Prerequisite check
        if (session.Course.PrerequisiteCourseId.HasValue)
        {
            var hasCompletion = await _context.TraineeCourseCompletions.AnyAsync(c =>
                c.TraineeId == trainee.TraineeId &&
                c.CourseId == session.Course.PrerequisiteCourseId.Value &&
                c.Result == "Pass");
            if (!hasCompletion)
            {
                var prereqTitle = session.Course.PrerequisiteCourse?.Title ?? "the prerequisite course";
                TempData["Error"] = $"You must complete {prereqTitle} before enrolling.";
                return RedirectToAction(nameof(Browse));
            }
        }

        // b) Capacity check
        if (session.CurrentEnrollment >= session.MaxCapacity)
        {
            TempData["Error"] = "Session is full. You may join the waitlist.";
            return RedirectToAction(nameof(Browse));
        }

        // c) Duplicate enrollment check
        var droppedStatus = await _context.EnrollmentStatuses.FirstOrDefaultAsync(s => s.Status == "Dropped");
        var droppedId = droppedStatus?.EnrollmentStatusId ?? -1;
        var alreadyEnrolled = await _context.Enrollments.AnyAsync(e =>
            e.SessionId == sessionId &&
            e.TraineeId == trainee.TraineeId &&
            e.EnrollmentStatusId != droppedId);
        if (alreadyEnrolled)
        {
            TempData["Error"] = "You are already enrolled in this session.";
            return RedirectToAction(nameof(Browse));
        }

        var enrolledStatus = await _context.EnrollmentStatuses.FirstOrDefaultAsync(s => s.Status == "Enrolled");
        if (enrolledStatus == null)
        {
            TempData["Error"] = "Enrollment status not configured. Please contact the administrator.";
            return RedirectToAction(nameof(Browse));
        }

        try
        {
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
            session.CurrentEnrollment++;
            session.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveEnrollmentUpdate",
                session.SessionId, session.CurrentEnrollment, session.MaxCapacity);

            TempData["Success"] = "You have successfully enrolled in the session.";
            return RedirectToAction(nameof(MyEnrollments));
        }
        catch
        {
            TempData["Error"] = "An error occurred while processing your enrollment. Please try again.";
            return RedirectToAction(nameof(Browse));
        }
    }

    // ── 3. Drop Enrollment — GET (confirmation form) ──────────────────────────
    [Authorize(Roles = AppRoles.Trainee)]
    public async Task<IActionResult> Drop(int enrollmentId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.UserId == userId);
        if (trainee == null) return Forbid();

        var enrollment = await _context.Enrollments
            .Include(e => e.EnrollmentStatus)
            .Include(e => e.Session).ThenInclude(s => s.Course)
            .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);

        if (enrollment == null) return NotFound();
        if (enrollment.TraineeId != trainee.TraineeId) return Forbid();

        if (enrollment.EnrollmentStatus.Status != "Enrolled" &&
            enrollment.EnrollmentStatus.Status != "Confirmed")
        {
            TempData["Error"] = "This enrollment cannot be dropped at its current status.";
            return RedirectToAction(nameof(MyEnrollments));
        }

        return View(new DropEnrollmentViewModel
        {
            EnrollmentId = enrollment.EnrollmentId,
            CourseTitle = enrollment.Session.Course.Title,
            SessionDate = enrollment.Session.SessionDate
        });
    }

    // ── 3. Drop Enrollment — POST ─────────────────────────────────────────────
    [HttpPost, ActionName("Drop")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.Trainee)]
    public async Task<IActionResult> DropConfirmed(DropEnrollmentViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.UserId == userId);
        if (trainee == null) return Forbid();

        var enrollment = await _context.Enrollments
            .Include(e => e.EnrollmentStatus)
            .Include(e => e.Session).ThenInclude(s => s.Course)
            .FirstOrDefaultAsync(e => e.EnrollmentId == vm.EnrollmentId);

        if (enrollment == null) return NotFound();
        if (enrollment.TraineeId != trainee.TraineeId) return Forbid();

        if (enrollment.EnrollmentStatus.Status != "Enrolled" &&
            enrollment.EnrollmentStatus.Status != "Confirmed")
        {
            TempData["Error"] = "This enrollment cannot be dropped at its current status.";
            return RedirectToAction(nameof(MyEnrollments));
        }

        var droppedStatus = await _context.EnrollmentStatuses.FirstOrDefaultAsync(s => s.Status == "Dropped");
        if (droppedStatus == null)
        {
            TempData["Error"] = "Status configuration error. Please contact the administrator.";
            return RedirectToAction(nameof(MyEnrollments));
        }

        try
        {
            enrollment.EnrollmentStatusId = droppedStatus.EnrollmentStatusId;
            enrollment.DropReason = vm.DropReason;
            enrollment.StatusChangedAt = DateTime.Now;
            enrollment.UpdatedAt = DateTime.Now;

            var session = enrollment.Session;
            session.CurrentEnrollment = Math.Max(0, session.CurrentEnrollment - 1);
            session.UpdatedAt = DateTime.Now;

            await PromoteFromWaitlistAsync(session, enrollment.Session.Course.Title);

            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveEnrollmentUpdate",
                session.SessionId, session.CurrentEnrollment, session.MaxCapacity);

            // Notify the coordinator who owns the session
            var coordinator = await _context.Coordinators
                .FirstOrDefaultAsync(c => c.CoordinatorId == session.CoordinatorId);
            if (coordinator != null)
            {
                var traineeUser = await _context.Users.FindAsync(trainee.UserId);
                await NotificationHelper.CreateAsync(_context, coordinator.UserId,
                    "Enrollment Dropped",
                    $"{traineeUser?.UserName ?? "A trainee"} has dropped their enrollment in {enrollment.Session.Course.Title} on {session.SessionDate:MMM dd, yyyy}.",
                    "Enrollment", "Enrollment");
            }

            TempData["Success"] = "Your enrollment has been dropped successfully.";
            return RedirectToAction(nameof(MyEnrollments));
        }
        catch
        {
            TempData["Error"] = "An error occurred while dropping the enrollment. Please try again.";
            return RedirectToAction(nameof(MyEnrollments));
        }
    }

    // ── 4. Join Waitlist ─────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.Trainee)]
    [ActionName("Waitlist")]
    public async Task<IActionResult> JoinWaitlist(int sessionId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.UserId == userId);
        if (trainee == null)
        {
            TempData["Error"] = "Trainee profile not found. Please contact the administrator.";
            return RedirectToAction(nameof(Browse));
        }

        var session = await _context.CourseSessions
            .Include(s => s.Course).ThenInclude(c => c.PrerequisiteCourse)
            .Include(s => s.Status)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session == null) return NotFound();

        if (session.Status.Status != "Scheduled" || session.SessionDate.Date < DateTime.Today)
        {
            TempData["Error"] = "This session is not available for waitlisting.";
            return RedirectToAction(nameof(Browse));
        }

        if (session.CurrentEnrollment < session.MaxCapacity)
        {
            TempData["Error"] = "This session still has available spots. Please enroll directly.";
            return RedirectToAction(nameof(Browse));
        }

        var alreadyWaiting = await _context.Waitlists
            .AnyAsync(w => w.SessionId == sessionId && w.TraineeId == trainee.TraineeId);
        if (alreadyWaiting)
        {
            TempData["Error"] = "You are already on the waitlist for this session.";
            return RedirectToAction(nameof(MyEnrollments));
        }

        // Prerequisite check
        if (session.Course.PrerequisiteCourseId.HasValue)
        {
            var hasCompletion = await _context.TraineeCourseCompletions.AnyAsync(c =>
                c.TraineeId == trainee.TraineeId &&
                c.CourseId == session.Course.PrerequisiteCourseId.Value &&
                c.Result == "Pass");
            if (!hasCompletion)
            {
                var prereqTitle = session.Course.PrerequisiteCourse?.Title ?? "the prerequisite course";
                TempData["Error"] = $"You must complete {prereqTitle} before joining the waitlist.";
                return RedirectToAction(nameof(Browse));
            }
        }

        var waitingStatus = await _context.WaitlistStatuses.FirstOrDefaultAsync(s => s.Status == "Waiting");
        if (waitingStatus == null)
        {
            TempData["Error"] = "Waitlist status not configured. Please contact the administrator.";
            return RedirectToAction(nameof(Browse));
        }

        try
        {
            var maxPosition = await _context.Waitlists
                .Where(w => w.SessionId == sessionId)
                .MaxAsync(w => (int?)w.Position) ?? 0;

            _context.Waitlists.Add(new Waitlist
            {
                SessionId = sessionId,
                TraineeId = trainee.TraineeId,
                StatusId = waitingStatus.StatusId,
                Position = maxPosition + 1,
                AddedAt = DateTime.Now,
                Status = "Waiting",
                ExpiresAt = session.SessionDate.AddDays(-2)
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = $"You have been added to the waitlist at position {maxPosition + 1}.";
            return RedirectToAction(nameof(MyEnrollments));
        }
        catch
        {
            TempData["Error"] = "An error occurred while joining the waitlist. Please try again.";
            return RedirectToAction(nameof(Browse));
        }
    }

    // ── 5. My Enrollments ────────────────────────────────────────────────────
    [Authorize(Roles = AppRoles.Trainee)]
    public async Task<IActionResult> MyEnrollments()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.UserId == userId);
        if (trainee == null)
        {
            TempData["Error"] = "Trainee profile not found.";
            return View(new MyEnrollmentsViewModel());
        }

        var today = DateTime.Today;

        var items = await _context.Enrollments
            .Where(e => e.TraineeId == trainee.TraineeId)
            .Select(e => new EnrollmentItemViewModel
            {
                EnrollmentId     = e.EnrollmentId,
                SessionId        = e.SessionId,
                CourseTitle      = e.Session.Course.Title,
                InstructorName   = e.Session.Instructor.User.UserName ?? $"Instructor {e.Session.InstructorId}",
                SessionDate      = e.Session.SessionDate,
                StartTime        = e.Session.StartTime,
                Status           = e.EnrollmentStatus.Status,
                AssessmentResult = e.Assessments.Select(a => a.Result).FirstOrDefault(),
                CanDrop          = e.EnrollmentStatus.Status == "Enrolled" || e.EnrollmentStatus.Status == "Confirmed"
            })
            .ToListAsync();

        var waitlistEntries = await _context.Waitlists
            .Where(w => w.TraineeId == trainee.TraineeId && w.Status != "Expired")
            .OrderBy(w => w.Position)
            .Select(w => new WaitlistItemViewModel
            {
                WaitlistId  = w.WaitlistId,
                SessionId   = w.SessionId,
                CourseTitle = w.Session.Course.Title,
                SessionDate = w.Session.SessionDate,
                Position    = w.Position,
                Status      = w.Status
            })
            .ToListAsync();

        var vm = new MyEnrollmentsViewModel
        {
            UpcomingEnrollments = items.Where(e => e.Status != "Dropped" && e.SessionDate >= today).ToList(),
            PastEnrollments     = items.Where(e => e.Status != "Dropped" && e.SessionDate < today).ToList(),
            DroppedEnrollments  = items.Where(e => e.Status == "Dropped").ToList(),
            WaitlistEntries     = waitlistEntries
        };

        return View(vm);
    }

    // ── 6. Manage All Enrollments (Coordinator) ───────────────────────────────
    [Authorize(Roles = AppRoles.Coordinator)]
    public async Task<IActionResult> Manage(
        int? filterSessionId, int? filterStatusId,
        string? filterTraineeName, string? dateFrom, string? dateTo)
    {
        // Step 1: get the "Cancelled" status ID from the lookup table (no navigation).
        // Step 2: get all session IDs with that status (plain FK comparison).
        // This mirrors the exact pattern used in SessionController.Index and is
        // the most reliable way to exclude cancelled sessions in EF Core.
        var cancelledCssStatus = await _context.CourseSessionStatuses
            .FirstOrDefaultAsync(s => s.Status == "Cancelled");

        var cancelledSessionIds = cancelledCssStatus != null
            ? await _context.CourseSessions
                .Where(s => s.StatusId == cancelledCssStatus.StatusId)
                .Select(s => s.SessionId)
                .ToListAsync()
            : new List<int>();

        var query = _context.Enrollments
            .Include(e => e.EnrollmentStatus)
            .Include(e => e.Session).ThenInclude(s => s.Course)
            .Include(e => e.Session).ThenInclude(s => s.Status)
            .Include(e => e.Trainee).ThenInclude(t => t.User)
            .Where(e => cancelledCssStatus == null || e.Session.StatusId != cancelledCssStatus.StatusId)
            .AsQueryable();

        if (filterSessionId.HasValue)
            query = query.Where(e => e.SessionId == filterSessionId.Value);
        if (filterStatusId.HasValue)
            query = query.Where(e => e.EnrollmentStatusId == filterStatusId.Value);
        if (!string.IsNullOrWhiteSpace(dateFrom) && DateTime.TryParse(dateFrom, out var from))
            query = query.Where(e => e.Session.SessionDate >= from);
        if (!string.IsNullOrWhiteSpace(dateTo) && DateTime.TryParse(dateTo, out var to))
            query = query.Where(e => e.Session.SessionDate <= to);

        var enrollments = await query.OrderByDescending(e => e.EnrollmentDate).ToListAsync();

        // Resolve trainee display names from already-loaded User navigation — no extra DB call
        var traineeUsers = enrollments
            .Select(e => e.Trainee)
            .DistinctBy(t => t.TraineeId)
            .ToDictionary(t => t.UserId, t => t.User?.UserName ?? $"Trainee {t.TraineeId}");

        // Check which enrollments already have a payment record — pull to memory to avoid CTE
        var enrollmentIds = enrollments.Select(e => e.EnrollmentId).ToList();
        var allPaymentRecords = await _context.PaymentRecords.ToListAsync();
        var paymentRecordMap = allPaymentRecords
            .Where(p => enrollmentIds.Contains(p.EnrollmentId))
            .ToDictionary(p => p.EnrollmentId, p => p.PaymentRecordId);
        var paidEnrollmentIds = paymentRecordMap.Keys.ToHashSet();

        var items = enrollments.Select(e => new ManageEnrollmentItemViewModel
        {
            EnrollmentId = e.EnrollmentId,
            TraineeName = traineeUsers.TryGetValue(e.Trainee.UserId, out var n) ? n : $"Trainee {e.TraineeId}",
            CourseTitle = e.Session.Course.Title,
            SessionDate = e.Session.SessionDate,
            SessionId = e.SessionId,
            Status = e.EnrollmentStatus.Status,
            EnrollmentDate = e.EnrollmentDate,
            CanConfirm = e.EnrollmentStatus.Status == "Enrolled"
                         && !cancelledSessionIds.Contains(e.SessionId)
                         && e.Session.Status?.Status != "Completed",
            HasPaymentRecord = paidEnrollmentIds.Contains(e.EnrollmentId),
            PaymentRecordId = paymentRecordMap.TryGetValue(e.EnrollmentId, out var prId) ? prId : null
        }).ToList();

        // Trainee name filter is applied in-memory (name lives in Identity, not in EF)
        if (!string.IsNullOrWhiteSpace(filterTraineeName))
            items = items.Where(i => i.TraineeName.Contains(filterTraineeName, StringComparison.OrdinalIgnoreCase)).ToList();

        var sessions = await _context.CourseSessions
            .Include(s => s.Course)
            .Include(s => s.Status)
            .Where(s => cancelledCssStatus == null || s.StatusId != cancelledCssStatus.StatusId)
            .OrderBy(s => s.SessionDate)
            .ToListAsync();

        var vm = new ManageEnrollmentsViewModel
        {
            Enrollments = items,
            FilterSessionId = filterSessionId,
            FilterStatusId = filterStatusId,
            FilterTraineeName = filterTraineeName,
            DateFrom = dateFrom,
            DateTo = dateTo,
            Sessions = sessions
                .Select(s => new SelectListItem
                {
                    Value = s.SessionId.ToString(),
                    Text = $"{s.Course.Title} – {s.SessionDate:MMM dd, yyyy}"
                }).ToList(),
            Statuses = await _context.EnrollmentStatuses
                .Select(s => new SelectListItem { Value = s.EnrollmentStatusId.ToString(), Text = s.Status })
                .ToListAsync(),
            SessionSummaries = sessions.Select(s => new SessionCapacitySummaryViewModel
            {
                SessionId = s.SessionId,
                CourseTitle = s.Course.Title,
                SessionDate = s.SessionDate,
                CurrentEnrollment = s.CurrentEnrollment,
                MaxCapacity = s.MaxCapacity
            }).ToList()
        };

        return View(vm);
    }

    // ── 7. Confirm Enrollment (Coordinator) ───────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.Coordinator)]
    public async Task<IActionResult> Confirm(int enrollmentId)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.EnrollmentStatus)
            .Include(e => e.Session).ThenInclude(s => s.Status)
            .Include(e => e.Session).ThenInclude(s => s.Course)
            .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);

        if (enrollment == null) return NotFound();

        if (enrollment.EnrollmentStatus.Status != "Enrolled")
        {
            TempData["Error"] = "Only enrollments in 'Enrolled' status can be confirmed.";
            return RedirectToAction(nameof(Manage));
        }

        var sessionStatus = enrollment.Session.Status?.Status;
        if (sessionStatus == "Cancelled" || sessionStatus == "Completed")
        {
            TempData["Error"] = "Cannot confirm enrollment for a cancelled or completed session.";
            return RedirectToAction(nameof(Manage));
        }

        var confirmedStatus = await _context.EnrollmentStatuses.FirstOrDefaultAsync(s => s.Status == "Confirmed");
        if (confirmedStatus == null)
        {
            TempData["Error"] = "Status configuration error. Please contact the administrator.";
            return RedirectToAction(nameof(Manage));
        }

        try
        {
            enrollment.EnrollmentStatusId = confirmedStatus.EnrollmentStatusId;
            enrollment.StatusChangedAt = DateTime.Now;
            enrollment.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            // Notify the trainee
            var trainee = await _context.Trainees
                .FirstOrDefaultAsync(t => t.TraineeId == enrollment.TraineeId);
            if (trainee != null)
            {
                await NotificationHelper.CreateAsync(_context, trainee.UserId,
                    "Enrollment Confirmed",
                    $"Your enrollment in {enrollment.Session.Course.Title} on {enrollment.Session.SessionDate:MMM dd, yyyy} has been confirmed.",
                    "Enrollment", "Enrollment");
            }

            TempData["Success"] = "Enrollment confirmed successfully.";
        }
        catch
        {
            TempData["Error"] = "An error occurred while confirming the enrollment.";
        }

        return RedirectToAction(nameof(Manage));
    }

    // ── 8. Mark Session as Attending (Coordinator) ────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.Coordinator)]
    public async Task<IActionResult> MarkAttending(int sessionId)
    {
        var session = await _context.CourseSessions
            .Include(s => s.Status)
            .Include(s => s.Enrollments).ThenInclude(e => e.EnrollmentStatus)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session == null) return NotFound();

        if (session.SessionDate.Date > DateTime.Today)
        {
            TempData["Error"] = "Attendance can only be marked on or after the session date.";
            return RedirectToAction(nameof(Manage));
        }

        if (session.Status?.Status != "Scheduled")
        {
            TempData["Error"] = "Session must be in 'Scheduled' status to mark attendance.";
            return RedirectToAction(nameof(Manage));
        }

        var attendingStatus = await _context.EnrollmentStatuses.FirstOrDefaultAsync(s => s.Status == "Attending");
        if (attendingStatus == null)
        {
            TempData["Error"] = "Status configuration error. Please contact the administrator.";
            return RedirectToAction(nameof(Manage));
        }

        try
        {
            var confirmed = session.Enrollments
                .Where(e => e.EnrollmentStatus.Status == "Confirmed").ToList();

            foreach (var e in confirmed)
            {
                e.EnrollmentStatusId = attendingStatus.EnrollmentStatusId;
                e.StatusChangedAt = DateTime.Now;
                e.UpdatedAt = DateTime.Now;
            }
            await _context.SaveChangesAsync();
            TempData["Success"] = $"{confirmed.Count} enrollment(s) marked as Attending.";
        }
        catch
        {
            TempData["Error"] = "An error occurred while marking attendance.";
        }

        return RedirectToAction(nameof(Manage));
    }

    // ── Instructor: My Sessions (entry point from sidebar) ───────────────────
    [Authorize(Roles = AppRoles.Instructor)]
    public async Task<IActionResult> MySessions()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == userId);
        if (instructor == null)
        {
            TempData["Error"] = "Instructor profile not found. Please contact the administrator.";
            return View(new InstructorSessionsViewModel());
        }

        var sessions = await _context.CourseSessions
            .Include(s => s.Course)
            .Include(s => s.Status)
            .Include(s => s.Enrollments).ThenInclude(e => e.EnrollmentStatus)
            .Include(s => s.Enrollments).ThenInclude(e => e.Assessments)
            .Where(s => s.InstructorId == instructor.InstructorId)
            .OrderByDescending(s => s.SessionDate)
            .ToListAsync();

        var vm = new InstructorSessionsViewModel
        {
            Sessions = sessions.Select(s => new InstructorSessionItemViewModel
            {
                SessionId = s.SessionId,
                CourseTitle = s.Course.Title,
                SessionDate = s.SessionDate,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                SessionStatus = s.Status.Status,
                EnrollmentCount = s.Enrollments.Count(e =>
                    e.EnrollmentStatus.Status == "Confirmed" ||
                    e.EnrollmentStatus.Status == "Attending" ||
                    e.EnrollmentStatus.Status == "Completed"),
                HasPendingAssessments = s.Enrollments.Any(e =>
                    (e.EnrollmentStatus.Status == "Confirmed" || e.EnrollmentStatus.Status == "Attending")
                    && !e.Assessments.Any(a => a.Result == "Pass" || a.Result == "Fail"))
            }).ToList()
        };

        return View(vm);
    }

    // ── Waitlist promotion (FIFO) ────────────────────────────────────────────
    // Called whenever a confirmed/enrolled spot is freed. Finds the earliest-
    // position "Waiting" entry, creates a real Enrollment for that trainee,
    // restores CurrentEnrollment (net change = 0 when a spot is filled), removes
    // the waitlist entry, and notifies the trainee. All pending context changes
    // (including the caller's drop) are flushed together via NotificationHelper.
    private async Task PromoteFromWaitlistAsync(CourseSession session, string courseTitle)
    {
        var firstWaiting = await _context.Waitlists
            .Include(w => w.Trainee)
            .Where(w => w.SessionId == session.SessionId && w.Status == "Waiting")
            .OrderBy(w => w.Position)
            .FirstOrDefaultAsync();

        if (firstWaiting == null) return;

        var enrolledStatus = await _context.EnrollmentStatuses
            .FirstOrDefaultAsync(s => s.Status == "Enrolled");
        if (enrolledStatus == null) return;

        _context.Enrollments.Add(new Enrollment
        {
            SessionId          = session.SessionId,
            TraineeId          = firstWaiting.TraineeId,
            EnrollmentStatusId = enrolledStatus.EnrollmentStatusId,
            EnrollmentDate     = DateTime.Now,
            StatusChangedAt    = DateTime.Now,
            CreatedAt          = DateTime.Now,
            UpdatedAt          = DateTime.Now
        });

        // Restore the counter decremented by the drop (net effect: 0 change)
        session.CurrentEnrollment++;
        session.UpdatedAt = DateTime.Now;

        _context.Waitlists.Remove(firstWaiting);

        // NotificationHelper.CreateAsync calls SaveChangesAsync, flushing all pending changes above
        await NotificationHelper.CreateAsync(_context, firstWaiting.Trainee.UserId,
            "Auto-Enrolled",
            $"A spot opened in {courseTitle} on {session.SessionDate:MMM dd, yyyy}. " +
            $"You have been automatically enrolled from the waitlist (you were position {firstWaiting.Position}).",
            "Enrollment", "Waitlist");
    }

    // ── 10. Session Roster (Instructor) ──────────────────────────────────────
    [Authorize(Roles = AppRoles.Instructor)]
    public async Task<IActionResult> SessionRoster(int sessionId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == userId);
        if (instructor == null) return Forbid();

        var session = await _context.CourseSessions
            .Include(s => s.Course)
            .Include(s => s.Status)
            .Include(s => s.Enrollments).ThenInclude(e => e.EnrollmentStatus)
            .Include(s => s.Enrollments).ThenInclude(e => e.Trainee)
            .Include(s => s.Enrollments).ThenInclude(e => e.Assessments)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session == null) return NotFound();
        if (session.InstructorId != instructor.InstructorId) return Forbid();

        var traineeUsers = await _context.Users
            .ToDictionaryAsync(u => u.Id, u => u.UserName ?? u.Id);

        var vm = new SessionRosterViewModel
        {
            SessionId = session.SessionId,
            CourseTitle = session.Course.Title,
            SessionDate = session.SessionDate,
            SessionStatus = session.Status.Status,
            Trainees = session.Enrollments
                .Select(e => new RosterTraineeViewModel
                {
                    EnrollmentId = e.EnrollmentId,
                    TraineeName = traineeUsers.TryGetValue(e.Trainee.UserId, out var n)
                        ? n : $"Trainee {e.TraineeId}",
                    EnrollmentStatus = e.EnrollmentStatus.Status,
                    AssessmentResult = e.Assessments.FirstOrDefault()?.Result
                })
                .OrderBy(t => t.TraineeName)
                .ToList()
        };

        return View(vm);
    }
}
