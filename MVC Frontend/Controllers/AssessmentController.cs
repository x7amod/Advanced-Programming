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
    public class AssessmentController : Controller
    {
        private readonly TrainingInstituteDBContext _context;

        public AssessmentController(TrainingInstituteDBContext context)
        {
            _context = context;
        }

        // ══════════════════════════════════════════════════════
        // TRAINEE — My Assessments
        // ══════════════════════════════════════════════════════

        // Shows all assessments linked to the current trainee's enrollments
        [Authorize(Roles = AppRoles.Trainee)]
        public async Task<IActionResult> MyAssessments(string? result)
        {
            var userId  = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.UserId == userId);

            if (trainee == null)
            {
                TempData["Error"] = "Trainee profile not found.";
                return RedirectToAction("Index", "Home");
            }

            // Fetch enrollments for this trainee that have at least one assessment
            var query = _context.Assessments
                .Include(a => a.Enrollment)
                    .ThenInclude(e => e.Session)
                        .ThenInclude(s => s.Course)
                .Include(a => a.Enrollment.Trainee)
                .Include(a => a.Instructor)
                .Where(a => a.Enrollment.TraineeId == trainee.TraineeId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(result))
                query = query.Where(a => a.Result == result);

            var assessments = await query
                .OrderByDescending(a => a.AssessmentDate)
                .ToListAsync();

            // Batch-fetch instructor display names
            var instructorIds   = assessments.Select(a => a.Instructor.UserId).Distinct().ToList();
            var instructorUsers = await _context.Users
                .Where(u => instructorIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.UserName ?? u.Email ?? u.Id);

            var items = assessments.Select(a => new AssessmentListItemViewModel
            {
                AssessmentId   = a.AssessmentId,
                EnrollmentId   = a.EnrollmentId,
                TraineeId      = trainee.TraineeId,
                TraineeName    = User.Identity?.Name ?? string.Empty,
                CourseTitle    = a.Enrollment.Session.Course.Title,
                CourseCode     = a.Enrollment.Session.Course.CourseCode,
                SessionDate    = a.Enrollment.Session.SessionDate,
                InstructorName = instructorUsers.TryGetValue(a.Instructor.UserId, out var n) ? n : $"Instructor {a.InstructorId}",
                Result         = a.Result,
                Remarks        = a.Remarks,
                AssessmentDate = a.AssessmentDate
            }).ToList();

            ViewBag.SelectedResult = result;
            return View(items);
        }

        // ══════════════════════════════════════════════════════
        // INSTRUCTOR — Session Results
        // ══════════════════════════════════════════════════════

        // Shows a summary of all trainee results for a specific session the instructor teaches
        [Authorize(Roles = AppRoles.Instructor)]
        public async Task<IActionResult> SessionResults(int sessionId)
        {
            var userId     = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == userId);

            if (instructor == null) return Forbid();

            var session = await _context.CourseSessions
                .Include(s => s.Course)
                .Include(s => s.Status)
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.EnrollmentStatus)
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Trainee)
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Assessments)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null) return NotFound();

            // Instructors can only access their own sessions
            if (session.InstructorId != instructor.InstructorId)
                return Forbid();

            // Batch-fetch trainee display names
            var traineeUserIds = session.Enrollments.Select(e => e.Trainee.UserId).Distinct().ToList();
            var traineeUsers   = await _context.Users
                .Where(u => traineeUserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.UserName ?? u.Email ?? u.Id);

            // Build one result row per non-dropped enrollment
            var resultRows = session.Enrollments
                .Where(e => e.EnrollmentStatus.Status != "Dropped")
                .Select(e =>
                {
                    // Use the most recent assessment if multiple exist (shouldn't normally happen)
                    var latestAssessment = e.Assessments.OrderByDescending(a => a.AssessmentDate).FirstOrDefault();
                    return new TraineeResultItemViewModel
                    {
                        EnrollmentId     = e.EnrollmentId,
                        TraineeId        = e.TraineeId,
                        TraineeName      = traineeUsers.TryGetValue(e.Trainee.UserId, out var tn) ? tn : $"Trainee {e.TraineeId}",
                        EnrollmentStatus = e.EnrollmentStatus.Status,
                        AssessmentId     = latestAssessment?.AssessmentId,
                        Result           = latestAssessment?.Result ?? "Pending",
                        Remarks          = latestAssessment?.Remarks,
                        AssessmentDate   = latestAssessment?.AssessmentDate
                    };
                })
                .OrderBy(r => r.TraineeName)
                .ToList();

            var vm = new SessionResultsViewModel
            {
                SessionId     = session.SessionId,
                CourseTitle   = session.Course.Title,
                CourseCode    = session.Course.CourseCode,
                SessionDate   = session.SessionDate,
                SessionStatus = session.Status.Status,
                Results       = resultRows,
                TotalEnrolled = resultRows.Count,
                PassCount     = resultRows.Count(r => r.Result == "Pass"),
                FailCount     = resultRows.Count(r => r.Result == "Fail"),
                PendingCount  = resultRows.Count(r => r.Result == "Pending")
            };

            return View(vm);
        }

        // ══════════════════════════════════════════════════════
        // COORDINATOR — All Assessments
        // ══════════════════════════════════════════════════════

        // Full assessment list with optional filters: session, instructor, result
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> AllAssessments(int? sessionId, int? instructorId, string? result)
        {
            var query = _context.Assessments
                .Include(a => a.Enrollment)
                    .ThenInclude(e => e.Session)
                        .ThenInclude(s => s.Course)
                .Include(a => a.Enrollment.Trainee)
                .Include(a => a.Instructor)
                .AsQueryable();

            if (sessionId.HasValue)
                query = query.Where(a => a.Enrollment.SessionId == sessionId.Value);
            if (instructorId.HasValue)
                query = query.Where(a => a.InstructorId == instructorId.Value);
            if (!string.IsNullOrWhiteSpace(result))
                query = query.Where(a => a.Result == result);

            var assessments = await query
                .OrderByDescending(a => a.AssessmentDate)
                .ToListAsync();

            // Batch-fetch both trainee and instructor display names
            var traineeUserIds   = assessments.Select(a => a.Enrollment.Trainee.UserId).Distinct().ToList();
            var instructorUserIds = assessments.Select(a => a.Instructor.UserId).Distinct().ToList();
            var allUserIds       = traineeUserIds.Union(instructorUserIds).ToList();

            var userMap = await _context.Users
                .Where(u => allUserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.UserName ?? u.Email ?? u.Id);

            var items = assessments.Select(a => new AssessmentListItemViewModel
            {
                AssessmentId   = a.AssessmentId,
                EnrollmentId   = a.EnrollmentId,
                TraineeId      = a.Enrollment.TraineeId,
                TraineeName    = userMap.TryGetValue(a.Enrollment.Trainee.UserId, out var tn) ? tn : $"Trainee {a.Enrollment.TraineeId}",
                CourseTitle    = a.Enrollment.Session.Course.Title,
                CourseCode     = a.Enrollment.Session.Course.CourseCode,
                SessionDate    = a.Enrollment.Session.SessionDate,
                InstructorName = userMap.TryGetValue(a.Instructor.UserId, out var iname) ? iname : $"Instructor {a.InstructorId}",
                Result         = a.Result,
                Remarks        = a.Remarks,
                AssessmentDate = a.AssessmentDate
            }).ToList();

            // Build instructor dropdown for filter bar
            var allInstructors  = await _context.Instructors.ToListAsync();
            var instrUserIds    = allInstructors.Select(i => i.UserId).ToList();
            var instrUsers      = await _context.Users
                .Where(u => instrUserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.UserName ?? u.Email ?? u.Id);

            var vm = new AllAssessmentsViewModel
            {
                Assessments       = items,
                FilterSessionId   = sessionId,
                FilterInstructorId = instructorId,
                FilterResult      = result,
                Sessions          = await _context.CourseSessions
                    .Include(s => s.Course)
                    .OrderBy(s => s.SessionDate)
                    .Select(s => new SelectListItem
                    {
                        Value = s.SessionId.ToString(),
                        Text  = $"{s.Course.CourseCode} — {s.SessionDate:dd MMM yyyy}"
                    })
                    .ToListAsync(),
                Instructors = allInstructors
                    .Select(i => new SelectListItem
                    {
                        Value = i.InstructorId.ToString(),
                        Text  = instrUsers.TryGetValue(i.UserId, out var n) ? n : $"Instructor {i.InstructorId}"
                    })
                    .OrderBy(x => x.Text)
                    .ToList()
            };

            return View(vm);
        }

        // ══════════════════════════════════════════════════════
        // COORDINATOR — Assessment Details
        // ══════════════════════════════════════════════════════

        // Read-only detail page accessible by trainee (own), instructor (own session), and coordinator
        public async Task<IActionResult> Details(int id)
        {
            var assessment = await _context.Assessments
                .Include(a => a.Enrollment)
                    .ThenInclude(e => e.Session)
                        .ThenInclude(s => s.Course)
                .Include(a => a.Enrollment.Trainee)
                .Include(a => a.Enrollment.EnrollmentStatus)
                .Include(a => a.Instructor)
                .FirstOrDefaultAsync(a => a.AssessmentId == id);

            if (assessment == null)
                return NotFound();

            // Trainee may only see their own assessment
            if (User.IsInRole(AppRoles.Trainee))
            {
                var userId  = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.UserId == userId);
                if (trainee == null || assessment.Enrollment.TraineeId != trainee.TraineeId)
                    return Forbid();
            }

            // Instructor may only see assessments for sessions they teach
            if (User.IsInRole(AppRoles.Instructor))
            {
                var userId     = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == userId);
                if (instructor == null || assessment.Enrollment.Session.InstructorId != instructor.InstructorId)
                    return Forbid();
            }

            var traineeUser    = await _context.Users.FindAsync(assessment.Enrollment.Trainee.UserId);
            var instructorUser = await _context.Users.FindAsync(assessment.Instructor.UserId);

            var vm = new AssessmentDetailsViewModel
            {
                AssessmentId     = assessment.AssessmentId,
                EnrollmentId     = assessment.EnrollmentId,
                TraineeId        = assessment.Enrollment.TraineeId,
                TraineeName      = traineeUser?.UserName ?? $"Trainee {assessment.Enrollment.TraineeId}",
                TraineeEmail     = traineeUser?.Email ?? string.Empty,
                CourseTitle      = assessment.Enrollment.Session.Course.Title,
                CourseCode       = assessment.Enrollment.Session.Course.CourseCode,
                SessionDate      = assessment.Enrollment.Session.SessionDate,
                InstructorName   = instructorUser?.UserName ?? $"Instructor {assessment.InstructorId}",
                Result           = assessment.Result,
                Remarks          = assessment.Remarks,
                AssessmentDate   = assessment.AssessmentDate,
                CreatedAt        = assessment.CreatedAt,
                EnrollmentStatus = assessment.Enrollment.EnrollmentStatus.Status
            };

            return View(vm);
        }

        // ══════════════════════════════════════════════════════
        // COORDINATOR — Create Assessment
        // ══════════════════════════════════════════════════════

        // Creates a new assessment record; enrollmentId pre-selects the enrollment if coming from an enrollment detail page
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> Create(int? enrollmentId)
        {
            var vm = new AssessmentFormViewModel
            {
                AssessmentDate = DateTime.Today,
                Result         = "Pending"
            };

            if (enrollmentId.HasValue)
            {
                vm.EnrollmentId = enrollmentId.Value;
                await PopulateEnrollmentContextAsync(vm);
            }

            await PopulateFormDropdownsAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> Create(AssessmentFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateEnrollmentContextAsync(vm);
                await PopulateFormDropdownsAsync(vm);
                return View(vm);
            }

            // Prevent creating a duplicate assessment for the same enrollment
            var duplicate = await _context.Assessments.AnyAsync(a => a.EnrollmentId == vm.EnrollmentId);
            if (duplicate)
            {
                ModelState.AddModelError(string.Empty, "An assessment already exists for this enrollment. Use Edit to update it.");
                await PopulateEnrollmentContextAsync(vm);
                await PopulateFormDropdownsAsync(vm);
                return View(vm);
            }

            var assessment = new Assessment
            {
                EnrollmentId   = vm.EnrollmentId,
                InstructorId   = vm.InstructorId,
                Result         = vm.Result,
                Remarks        = vm.Remarks,
                AssessmentDate = vm.AssessmentDate,
                CreatedAt      = DateTime.Now
            };

            _context.Assessments.Add(assessment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Assessment created successfully.";
            return RedirectToAction(nameof(Details), new { id = assessment.AssessmentId });
        }

        // ══════════════════════════════════════════════════════
        // COORDINATOR — Edit Assessment
        // ══════════════════════════════════════════════════════

        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> Edit(int id)
        {
            var assessment = await _context.Assessments
                .Include(a => a.Enrollment)
                    .ThenInclude(e => e.Session)
                        .ThenInclude(s => s.Course)
                .Include(a => a.Enrollment.Trainee)
                .FirstOrDefaultAsync(a => a.AssessmentId == id);

            if (assessment == null)
                return NotFound();

            var traineeUser = await _context.Users.FindAsync(assessment.Enrollment.Trainee.UserId);

            var vm = new AssessmentFormViewModel
            {
                AssessmentId   = assessment.AssessmentId,
                EnrollmentId   = assessment.EnrollmentId,
                InstructorId   = assessment.InstructorId,
                Result         = assessment.Result,
                Remarks        = assessment.Remarks,
                AssessmentDate = assessment.AssessmentDate,
                TraineeName    = traineeUser?.UserName ?? $"Trainee {assessment.Enrollment.TraineeId}",
                CourseTitle    = assessment.Enrollment.Session.Course.Title,
                CourseCode     = assessment.Enrollment.Session.Course.CourseCode,
                SessionDate    = assessment.Enrollment.Session.SessionDate
            };

            await PopulateFormDropdownsAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> Edit(int id, AssessmentFormViewModel vm)
        {
            if (id != vm.AssessmentId)
                return BadRequest();

            var assessment = await _context.Assessments
                .Include(a => a.Enrollment)
                    .ThenInclude(e => e.Session)
                        .ThenInclude(s => s.Course)
                .Include(a => a.Enrollment.Trainee)
                .FirstOrDefaultAsync(a => a.AssessmentId == id);

            if (assessment == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                var traineeUser = await _context.Users.FindAsync(assessment.Enrollment.Trainee.UserId);
                vm.TraineeName  = traineeUser?.UserName ?? $"Trainee {assessment.Enrollment.TraineeId}";
                vm.CourseTitle  = assessment.Enrollment.Session.Course.Title;
                vm.CourseCode   = assessment.Enrollment.Session.Course.CourseCode;
                vm.SessionDate  = assessment.Enrollment.Session.SessionDate;
                await PopulateFormDropdownsAsync(vm);
                return View(vm);
            }

            assessment.InstructorId   = vm.InstructorId;
            assessment.Result         = vm.Result;
            assessment.Remarks        = vm.Remarks;
            assessment.AssessmentDate = vm.AssessmentDate;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Assessment updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // ══════════════════════════════════════════════════════
        // COORDINATOR — Delete Assessment
        // ══════════════════════════════════════════════════════

        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> Delete(int id)
        {
            var assessment = await _context.Assessments
                .Include(a => a.Enrollment)
                    .ThenInclude(e => e.Session)
                        .ThenInclude(s => s.Course)
                .Include(a => a.Enrollment.Trainee)
                .Include(a => a.Instructor)
                .FirstOrDefaultAsync(a => a.AssessmentId == id);

            if (assessment == null)
                return NotFound();

            var traineeUser    = await _context.Users.FindAsync(assessment.Enrollment.Trainee.UserId);
            var instructorUser = await _context.Users.FindAsync(assessment.Instructor.UserId);

            var vm = new AssessmentDetailsViewModel
            {
                AssessmentId   = assessment.AssessmentId,
                EnrollmentId   = assessment.EnrollmentId,
                TraineeId      = assessment.Enrollment.TraineeId,
                TraineeName    = traineeUser?.UserName ?? $"Trainee {assessment.Enrollment.TraineeId}",
                CourseTitle    = assessment.Enrollment.Session.Course.Title,
                CourseCode     = assessment.Enrollment.Session.Course.CourseCode,
                SessionDate    = assessment.Enrollment.Session.SessionDate,
                InstructorName = instructorUser?.UserName ?? $"Instructor {assessment.InstructorId}",
                Result         = assessment.Result,
                Remarks        = assessment.Remarks,
                AssessmentDate = assessment.AssessmentDate,
                CreatedAt      = assessment.CreatedAt
            };

            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assessment = await _context.Assessments.FindAsync(id);
            if (assessment == null)
                return NotFound();

            _context.Assessments.Remove(assessment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Assessment deleted successfully.";
            return RedirectToAction(nameof(AllAssessments));
        }

        // ══════════════════════════════════════════════════════
        // Helpers
        // ══════════════════════════════════════════════════════

        // Fills the instructor dropdown on the assessment form
        private async Task PopulateFormDropdownsAsync(AssessmentFormViewModel vm)
        {
            var instructors  = await _context.Instructors.ToListAsync();
            var instrUserIds = instructors.Select(i => i.UserId).ToList();
            var instrUsers   = await _context.Users
                .Where(u => instrUserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.UserName ?? u.Email ?? u.Id);

            vm.Instructors = instructors
                .Select(i => new SelectListItem
                {
                    Value = i.InstructorId.ToString(),
                    Text  = instrUsers.TryGetValue(i.UserId, out var n) ? n : $"Instructor {i.InstructorId}"
                })
                .OrderBy(x => x.Text)
                .ToList();

            // Enrollments dropdown is only needed when creating without a pre-selected enrollment
            if (vm.EnrollmentId == 0)
            {
                vm.Enrollments = await _context.Enrollments
                    .Include(e => e.Session)
                        .ThenInclude(s => s.Course)
                    .Include(e => e.Trainee)
                    .Where(e => !_context.Assessments.Any(a => a.EnrollmentId == e.EnrollmentId))
                    .OrderBy(e => e.Session.Course.Title)
                    .Select(e => new SelectListItem
                    {
                        Value = e.EnrollmentId.ToString(),
                        Text  = $"{e.Session.Course.CourseCode} — Trainee #{e.TraineeId} ({e.Session.SessionDate:dd MMM yyyy})"
                    })
                    .ToListAsync();
            }
        }

        // Loads trainee/course display fields for an existing enrollment (used on the Create form)
        private async Task PopulateEnrollmentContextAsync(AssessmentFormViewModel vm)
        {
            if (vm.EnrollmentId == 0) return;

            var enrollment = await _context.Enrollments
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Include(e => e.Trainee)
                .FirstOrDefaultAsync(e => e.EnrollmentId == vm.EnrollmentId);

            if (enrollment == null) return;

            var traineeUser     = await _context.Users.FindAsync(enrollment.Trainee.UserId);
            vm.TraineeName      = traineeUser?.UserName ?? $"Trainee {enrollment.TraineeId}";
            vm.CourseTitle      = enrollment.Session.Course.Title;
            vm.CourseCode       = enrollment.Session.Course.CourseCode;
            vm.SessionDate      = enrollment.Session.SessionDate;
        }
    }
}
