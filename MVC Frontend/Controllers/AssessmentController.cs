using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Frontend.Helpers;
using MVC_Frontend.Models;
using Web_API.Models;

namespace MVC_Frontend.Controllers;

public class AssessmentController : Controller
{
    private readonly TrainingInstituteDBContext _context;

    public AssessmentController(TrainingInstituteDBContext context)
    {
        _context = context;
    }

    // ── 11. GET Record Assessments (Instructor) ───────────────────────────────
    [Authorize(Roles = AppRoles.Instructor)]
    public async Task<IActionResult> Record(int sessionId)
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
        if (session.Status.Status != "Completed") return Forbid();

        // Only trainees who were Confirmed or Attending (i.e. the coordinator approved them).
        // Exclude plain "Enrolled" (not yet confirmed) and already Dropped / Completed.
        var assessableStatuses = new[] { "Confirmed", "Attending" };
        var eligible = session.Enrollments
            .Where(e => assessableStatuses.Contains(e.EnrollmentStatus.Status) && !e.Assessments.Any())
            .ToList();

        if (!eligible.Any())
        {
            TempData["Info"] = "All assessments for this session have already been submitted.";
            return RedirectToAction(nameof(MyAssessments));
        }

        var traineeUserIds = eligible.Select(e => e.Trainee.UserId).Distinct().ToList();
        var traineeUsers = await _context.Users
            .Where(u => traineeUserIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.UserName ?? u.Id);

        var vm = new RecordAssessmentViewModel
        {
            SessionId = sessionId,
            CourseTitle = session.Course.Title,
            SessionDate = session.SessionDate,
            Trainees = eligible.Select(e => new TraineeAssessmentRowViewModel
            {
                EnrollmentId = e.EnrollmentId,
                TraineeId = e.TraineeId,
                TraineeName = traineeUsers.TryGetValue(e.Trainee.UserId, out var n)
                    ? n : $"Trainee {e.TraineeId}"
            }).ToList()
        };

        return View(vm);
    }

    // ── 12. POST Record Assessments (Instructor) ──────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.Instructor)]
    public async Task<IActionResult> Record(int sessionId, RecordAssessmentViewModel vm)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == userId);
        if (instructor == null) return Forbid();

        // Re-verify ownership and status from DB
        var session = await _context.CourseSessions
            .Include(s => s.Course)
            .Include(s => s.Status)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session == null) return NotFound();
        if (session.InstructorId != instructor.InstructorId) return Forbid();
        if (session.Status.Status != "Completed") return Forbid();

        // Re-populate display fields for form re-render
        vm.SessionId = sessionId;
        vm.CourseTitle = session.Course.Title;
        vm.SessionDate = session.SessionDate;

        // Validate: every row must have a result selected
        if (vm.Trainees == null || !vm.Trainees.Any() ||
            vm.Trainees.Any(t => string.IsNullOrEmpty(t.Result)))
        {
            ModelState.AddModelError(string.Empty,
                "Please select a result for all trainees before submitting.");
            return View(vm);
        }

        // Prevent duplicate submissions
        var enrollmentIds = vm.Trainees.Select(t => t.EnrollmentId).ToList();
        var alreadyAssessed = await _context.Assessments
            .Where(a => enrollmentIds.Contains(a.EnrollmentId))
            .Select(a => a.EnrollmentId)
            .ToListAsync();

        if (alreadyAssessed.Any())
        {
            ModelState.AddModelError(string.Empty,
                "Some assessments have already been submitted. Please refresh the page and try again.");
            return View(vm);
        }

        var completedStatus = await _context.EnrollmentStatuses
            .FirstOrDefaultAsync(s => s.Status == "Completed");
        if (completedStatus == null)
        {
            TempData["Error"] = "Enrollment status configuration error. Please contact the administrator.";
            return View(vm);
        }

        try
        {
            var today = DateTime.Today;

            foreach (var row in vm.Trainees)
            {
                // a) Create Assessment record
                _context.Assessments.Add(new Assessment
                {
                    EnrollmentId = row.EnrollmentId,
                    InstructorId = instructor.InstructorId,
                    Result = row.Result!,
                    Remarks = string.IsNullOrWhiteSpace(row.Remarks) ? null : row.Remarks.Trim(),
                    AssessmentDate = today,
                    CreatedAt = DateTime.Now
                });

                // b) Update Enrollment status to Completed
                var enrollment = await _context.Enrollments.FindAsync(row.EnrollmentId);
                if (enrollment != null)
                {
                    enrollment.EnrollmentStatusId = completedStatus.EnrollmentStatusId;
                    enrollment.StatusChangedAt = DateTime.Now;
                    enrollment.UpdatedAt = DateTime.Now;
                }

                // c) Create TraineeCourseCompletion
                _context.TraineeCourseCompletions.Add(new TraineeCourseCompletion
                {
                    TraineeId = row.TraineeId,
                    CourseId = session.CourseId,
                    SessionId = sessionId,
                    CompletionDate = today,
                    Result = row.Result!
                });
            }

            await _context.SaveChangesAsync();

            // Notify each trainee of their result
            var traineeIds = vm.Trainees.Select(t => t.TraineeId).ToList();
            var trainees = await _context.Trainees
                .Where(t => traineeIds.Contains(t.TraineeId))
                .ToListAsync();
            var traineeUserIdMap = trainees.ToDictionary(t => t.TraineeId, t => t.UserId);

            foreach (var row in vm.Trainees)
            {
                if (traineeUserIdMap.TryGetValue(row.TraineeId, out var tUserId))
                {
                    await NotificationHelper.CreateAsync(_context, tUserId,
                        "Assessment Result Available",
                        $"Your result for {vm.CourseTitle} ({vm.SessionDate:MMM dd, yyyy}): {row.Result}.",
                        "Assessment", "Assessment");
                }
            }

            TempData["Success"] = "Assessment results submitted successfully.";
            return RedirectToAction(nameof(MyAssessments));
        }
        catch
        {
            TempData["Error"] = "An error occurred while saving results. Please try again.";
            return View(vm);
        }
    }

    // ── 13. My Submitted Assessments (Instructor) ────────────────────────────
    [Authorize(Roles = AppRoles.Instructor)]
    public async Task<IActionResult> MyAssessments(
        int? filterSessionId, string? filterResult, string? dateFrom, string? dateTo)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == userId);
        if (instructor == null)
        {
            TempData["Error"] = "Instructor profile not found.";
            return View(new MyAssessmentsViewModel());
        }

        var query = _context.Assessments
            .Include(a => a.Enrollment).ThenInclude(e => e.Session).ThenInclude(s => s.Course)
            .Include(a => a.Enrollment).ThenInclude(e => e.Trainee)
            .Where(a => a.InstructorId == instructor.InstructorId)
            .AsQueryable();

        if (filterSessionId.HasValue)
            query = query.Where(a => a.Enrollment.SessionId == filterSessionId.Value);
        if (!string.IsNullOrWhiteSpace(filterResult))
            query = query.Where(a => a.Result == filterResult);
        if (!string.IsNullOrWhiteSpace(dateFrom) && DateTime.TryParse(dateFrom, out var from))
            query = query.Where(a => a.AssessmentDate >= from);
        if (!string.IsNullOrWhiteSpace(dateTo) && DateTime.TryParse(dateTo, out var to))
            query = query.Where(a => a.AssessmentDate <= to);

        var assessments = await query.OrderByDescending(a => a.AssessmentDate).ToListAsync();

        var traineeUserIds = assessments.Select(a => a.Enrollment.Trainee.UserId).Distinct().ToList();
        var traineeUsers = await _context.Users
            .Where(u => traineeUserIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.UserName ?? u.Id);

        // Session filter dropdown — instructor's sessions only
        var instructorSessions = await _context.CourseSessions
            .Include(s => s.Course)
            .Where(s => s.InstructorId == instructor.InstructorId)
            .OrderByDescending(s => s.SessionDate)
            .ToListAsync();

        var vm = new MyAssessmentsViewModel
        {
            FilterSessionId = filterSessionId,
            FilterResult = filterResult,
            DateFrom = dateFrom,
            DateTo = dateTo,
            Sessions = instructorSessions.Select(s => new SelectListItem
            {
                Value = s.SessionId.ToString(),
                Text = $"{s.Course.Title} – {s.SessionDate:MMM dd, yyyy}"
            }).ToList(),
            Assessments = assessments.Select(a => new AssessmentItemViewModel
            {
                AssessmentId = a.AssessmentId,
                TraineeName = traineeUsers.TryGetValue(a.Enrollment.Trainee.UserId, out var n)
                    ? n : $"Trainee {a.Enrollment.TraineeId}",
                CourseTitle = a.Enrollment.Session.Course.Title,
                SessionDate = a.Enrollment.Session.SessionDate,
                SessionId = a.Enrollment.SessionId,
                Result = a.Result,
                Remarks = a.Remarks,
                AssessmentDate = a.AssessmentDate
            }).ToList()
        };

        return View(vm);
    }

    // ── 14. All Assessments (Coordinator) ────────────────────────────────────
    [Authorize(Roles = AppRoles.Coordinator)]
    public async Task<IActionResult> All(
        int? filterCourseId, int? filterInstructorId,
        string? filterResult, string? dateFrom, string? dateTo)
    {
        var query = _context.Assessments
            .Include(a => a.Enrollment).ThenInclude(e => e.Session).ThenInclude(s => s.Course)
            .Include(a => a.Enrollment).ThenInclude(e => e.Trainee)
            .Include(a => a.Instructor)
            .AsQueryable();

        if (filterCourseId.HasValue)
            query = query.Where(a => a.Enrollment.Session.CourseId == filterCourseId.Value);
        if (filterInstructorId.HasValue)
            query = query.Where(a => a.InstructorId == filterInstructorId.Value);
        if (!string.IsNullOrWhiteSpace(filterResult))
            query = query.Where(a => a.Result == filterResult);
        if (!string.IsNullOrWhiteSpace(dateFrom) && DateTime.TryParse(dateFrom, out var from))
            query = query.Where(a => a.AssessmentDate >= from);
        if (!string.IsNullOrWhiteSpace(dateTo) && DateTime.TryParse(dateTo, out var to))
            query = query.Where(a => a.AssessmentDate <= to);

        var assessments = await query.OrderByDescending(a => a.AssessmentDate).ToListAsync();

        // Collect all user IDs (trainees + instructors) in one DB call
        var traineeUserIds = assessments.Select(a => a.Enrollment.Trainee.UserId).Distinct().ToList();
        var instructorUserIds = assessments.Select(a => a.Instructor.UserId).Distinct().ToList();
        var allUserIds = traineeUserIds.Union(instructorUserIds).Distinct().ToList();
        var allUsers = await _context.Users
            .Where(u => allUserIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.UserName ?? u.Id);

        // Pass rate summary — calculated from the current filtered results
        var passRates = assessments
            .GroupBy(a => a.Enrollment.SessionId)
            .Select(g => new SessionPassRateViewModel
            {
                SessionId = g.Key,
                CourseTitle = g.First().Enrollment.Session.Course.Title,
                SessionDate = g.First().Enrollment.Session.SessionDate,
                Total = g.Count(),
                Passed = g.Count(a => a.Result == "Pass")
            })
            .OrderByDescending(p => p.SessionDate)
            .ToList();

        // Filter dropdowns
        var courses = await _context.Courses.OrderBy(c => c.Title)
            .Select(c => new SelectListItem { Value = c.CourseId.ToString(), Text = c.Title })
            .ToListAsync();

        var allInstructors = await _context.Instructors.ToListAsync();
        var iUserIds = allInstructors.Select(i => i.UserId).ToList();
        var iUsers = await _context.Users
            .Where(u => iUserIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.UserName ?? u.Id);

        var vm = new AllAssessmentsViewModel
        {
            FilterCourseId = filterCourseId,
            FilterInstructorId = filterInstructorId,
            FilterResult = filterResult,
            DateFrom = dateFrom,
            DateTo = dateTo,
            Courses = courses,
            Instructors = allInstructors
                .Select(i => new SelectListItem
                {
                    Value = i.InstructorId.ToString(),
                    Text = iUsers.TryGetValue(i.UserId, out var n) ? n : $"Instructor {i.InstructorId}"
                })
                .OrderBy(x => x.Text)
                .ToList(),
            PassRates = passRates,
            Assessments = assessments.Select(a => new AssessmentItemViewModel
            {
                AssessmentId = a.AssessmentId,
                TraineeName = allUsers.TryGetValue(a.Enrollment.Trainee.UserId, out var tn)
                    ? tn : $"Trainee {a.Enrollment.TraineeId}",
                InstructorName = allUsers.TryGetValue(a.Instructor.UserId, out var iname)
                    ? iname : $"Instructor {a.InstructorId}",
                CourseTitle = a.Enrollment.Session.Course.Title,
                SessionDate = a.Enrollment.Session.SessionDate,
                SessionId = a.Enrollment.SessionId,
                Result = a.Result,
                Remarks = a.Remarks,
                AssessmentDate = a.AssessmentDate
            }).ToList()
        };

        return View(vm);
    }

    // ── 15. My Results (Trainee) ─────────────────────────────────────────────
    [Authorize(Roles = AppRoles.Trainee)]
    public async Task<IActionResult> MyResults()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.UserId == userId);
        if (trainee == null)
        {
            TempData["Error"] = "Trainee profile not found.";
            return View(new MyResultsViewModel());
        }

        // Query through Assessment to get remarks alongside results
        var assessments = await _context.Assessments
            .Include(a => a.Enrollment).ThenInclude(e => e.Session).ThenInclude(s => s.Course)
            .Where(a => a.Enrollment.TraineeId == trainee.TraineeId)
            .OrderByDescending(a => a.AssessmentDate)
            .ToListAsync();

        var vm = new MyResultsViewModel
        {
            Results = assessments.Select(a => new TraineeResultItemViewModel
            {
                CourseTitle = a.Enrollment.Session.Course.Title,
                SessionDate = a.Enrollment.Session.SessionDate,
                Result = a.Result,
                Remarks = a.Remarks,
                CompletionDate = a.AssessmentDate
            }).ToList()
        };

        return View(vm);
    }
}
