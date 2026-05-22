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
    public class SessionController : Controller
    {
        private readonly TrainingInstituteDBContext _context;

        public SessionController(TrainingInstituteDBContext context)
        {
            _context = context;
        }

        // GET: Session
        public async Task<IActionResult> Index(int? courseId, int? statusId, int? instructorId, string? dateFrom, string? dateTo)
        {
            var query = _context.CourseSessions
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .Include(s => s.Classroom)
                .Include(s => s.Status)
                .AsQueryable();

            // Instructors only see their own sessions
            if (User.IsInRole(AppRoles.Instructor))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == userId);
                if (instructor != null)
                    query = query.Where(s => s.InstructorId == instructor.InstructorId);
            }

            // Trainees only see upcoming non-cancelled sessions
            if (User.IsInRole(AppRoles.Trainee))
            {
                query = query.Where(s => s.SessionDate >= DateTime.Today);
                var cancelledStatus = await _context.CourseSessionStatuses
                    .FirstOrDefaultAsync(st => st.Status == "Cancelled");
                if (cancelledStatus != null)
                    query = query.Where(s => s.StatusId != cancelledStatus.StatusId);
            }

            if (courseId.HasValue)
                query = query.Where(s => s.CourseId == courseId.Value);

            if (statusId.HasValue)
                query = query.Where(s => s.StatusId == statusId.Value);

            if (instructorId.HasValue && User.IsInRole(AppRoles.Coordinator))
                query = query.Where(s => s.InstructorId == instructorId.Value);

            if (!string.IsNullOrWhiteSpace(dateFrom) && DateTime.TryParse(dateFrom, out var from))
                query = query.Where(s => s.SessionDate >= from);

            if (!string.IsNullOrWhiteSpace(dateTo) && DateTime.TryParse(dateTo, out var to))
                query = query.Where(s => s.SessionDate <= to);

            ViewBag.Courses = new SelectList(
                await _context.Courses.Where(c => c.IsActive).OrderBy(c => c.Title).ToListAsync(),
                "CourseId", "Title", courseId);

            ViewBag.Statuses = new SelectList(
                await _context.CourseSessionStatuses.ToListAsync(),
                "StatusId", "Status", statusId);

            // Instructor filter only for coordinators
            if (User.IsInRole(AppRoles.Coordinator))
            {
                var allInstructors = await _context.Instructors.ToListAsync();
                var iUserIds = allInstructors.Select(i => i.UserId).ToList();
                var iUsersAll = await _context.Users.ToListAsync();
                var iUsers = iUsersAll
                    .Where(u => iUserIds.Contains(u.Id))
                    .ToDictionary(u => u.Id, u => u.UserName ?? $"User {u.Id}");
                var instructorItems = allInstructors
                    .Select(i => new SelectListItem
                    {
                        Value = i.InstructorId.ToString(),
                        Text = iUsers.TryGetValue(i.UserId, out var n) ? n : $"Instructor {i.InstructorId}"
                    })
                    .OrderBy(x => x.Text)
                    .ToList();
                ViewBag.Instructors = new SelectList(instructorItems, "Value", "Text", instructorId);
            }

            ViewBag.SelectedCourseId = courseId;
            ViewBag.SelectedStatusId = statusId;
            ViewBag.SelectedInstructorId = instructorId;
            ViewBag.DateFrom = dateFrom;
            ViewBag.DateTo = dateTo;

            var sessions = await query.OrderBy(s => s.SessionDate).ThenBy(s => s.StartTime).ToListAsync();

            // Build a map of instructorId -> display name for the list view
            var instructorIds = sessions.Select(s => s.InstructorId).Distinct().ToList();
            var sessionInstructors = await _context.Instructors
                .Where(i => instructorIds.Contains(i.InstructorId))
                .ToListAsync();
            var sessionUserIds = sessionInstructors.Select(i => i.UserId).ToList();
            var sessionUsersAll = await _context.Users.ToListAsync();
            var sessionUsers = sessionUsersAll
                .Where(u => sessionUserIds.Contains(u.Id))
                .ToDictionary(u => u.Id, u => u.UserName ?? $"User {u.Id}");
            var nameMap = sessionInstructors.ToDictionary(
                i => i.InstructorId,
                i => sessionUsers.TryGetValue(i.UserId, out var n) ? n : $"Instructor {i.InstructorId}");
            ViewBag.InstructorNames = nameMap;

            return View(sessions);
        }

        // GET: Session/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var session = await _context.CourseSessions
                .Include(s => s.Course)
                    .ThenInclude(c => c.SubjectArea)
                .Include(s => s.Instructor)
                .Include(s => s.Classroom)
                    .ThenInclude(c => c.ClassroomEquipments)
                .Include(s => s.Status)
                .Include(s => s.Coordinator)
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.EnrollmentStatus)
                .FirstOrDefaultAsync(s => s.SessionId == id);

            if (session == null)
                return NotFound();

            // Instructors can only view their own session details
            if (User.IsInRole(AppRoles.Instructor))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == userId);
                if (instructor == null || session.InstructorId != instructor.InstructorId)
                    return Forbid();
            }

            // Load the instructor's IdentityUser for display name
            var instructorUser = await _context.Users.FindAsync(session.Instructor.UserId);
            ViewBag.InstructorName = instructorUser?.UserName ?? $"Instructor {session.InstructorId}";

            var coordinatorUser = await _context.Users.FindAsync(session.Coordinator.UserId);
            ViewBag.CoordinatorName = coordinatorUser?.UserName ?? $"Coordinator {session.CoordinatorId}";

            return View(session);
        }

        // GET: Session/Schedule
        public async Task<IActionResult> Schedule(int weekOffset = 0)
        {
            var today = DateTime.Today;
            int daysSinceMonday = ((int)today.DayOfWeek - 1 + 7) % 7;
            var monday = today.AddDays(-daysSinceMonday + weekOffset * 7);
            var sunday = monday.AddDays(6);

            var query = _context.CourseSessions
                .Include(s => s.Course)
                .Include(s => s.Classroom)
                .Include(s => s.Instructor)
                .Include(s => s.Status)
                .Where(s => s.SessionDate >= monday && s.SessionDate <= sunday);

            if (User.IsInRole(AppRoles.Instructor))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == userId);
                if (instructor != null)
                    query = query.Where(s => s.InstructorId == instructor.InstructorId);
            }

            var sessions = await query.OrderBy(s => s.StartTime).ToListAsync();

            ViewBag.Monday = monday;
            ViewBag.Sunday = sunday;
            ViewBag.WeekOffset = weekOffset;
            ViewBag.Today = today;

            // Group by day of week (0=Mon ... 6=Sun)
            var grouped = new Dictionary<int, List<CourseSession>>();
            for (int i = 0; i < 7; i++)
                grouped[i] = new List<CourseSession>();

            foreach (var s in sessions)
            {
                int idx = ((int)s.SessionDate.DayOfWeek - 1 + 7) % 7;
                grouped[idx].Add(s);
            }

            ViewBag.Grouped = grouped;
            return View();
        }

        // GET: Session/Create
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> Create(int? courseId)
        {
            var vm = new SessionFormViewModel { StatusId = 1 };
            if (courseId.HasValue)
            {
                vm.CourseId = courseId.Value;
                var course = await _context.Courses.FindAsync(courseId.Value);
                if (course != null)
                    vm.MaxCapacity = course.MaxCapacity;
            }
            return View(await PopulateDropdowns(vm));
        }

        // POST: Session/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> Create(SessionFormViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(await PopulateDropdowns(vm));

            if (!TryParseTimesToDateTimes(vm, out var startDt, out var endDt, out var timeError))
            {
                ModelState.AddModelError(string.Empty, timeError);
                return View(await PopulateDropdowns(vm));
            }

            if (startDt <= DateTime.Now)
            {
                ModelState.AddModelError(string.Empty,
                    "Session start time must be in the future. Please choose a later date or time.");
                return View(await PopulateDropdowns(vm));
            }

            if (await HasInstructorConflict(vm.InstructorId, vm.SessionDate, startDt, endDt))
            {
                ModelState.AddModelError(string.Empty,
                    "This instructor already has a session scheduled during the selected time slot. Please choose a different time or instructor.");
                return View(await PopulateDropdowns(vm));
            }

            if (await HasClassroomConflict(vm.ClassroomId, vm.SessionDate, startDt, endDt))
            {
                ModelState.AddModelError(string.Empty,
                    "This classroom is already booked for another session during the selected time slot. Please choose a different classroom or time.");
                return View(await PopulateDropdowns(vm));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var coordinator = await _context.Coordinators.FirstOrDefaultAsync(c => c.UserId == userId);
            if (coordinator == null)
            {
                TempData["Error"] = "Coordinator profile not found. Please contact the system administrator.";
                return RedirectToAction(nameof(Index));
            }

            var session = new CourseSession
            {
                CourseId = vm.CourseId,
                InstructorId = vm.InstructorId,
                ClassroomId = vm.ClassroomId,
                CoordinatorId = coordinator.CoordinatorId,
                StatusId = vm.StatusId,
                SessionDate = vm.SessionDate.Date,
                StartTime = startDt,
                EndTime = endDt,
                MaxCapacity = vm.MaxCapacity,
                CurrentEnrollment = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.CourseSessions.Add(session);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Session scheduled successfully.";
            return RedirectToAction(nameof(Details), new { id = session.SessionId });
        }

        // GET: Session/Edit/5
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> Edit(int id)
        {
            var session = await _context.CourseSessions
                .Include(s => s.Status)
                .FirstOrDefaultAsync(s => s.SessionId == id);
            if (session == null)
                return NotFound();

            // Completed sessions are read-only — redirect back
            if (session.Status?.Status == "Completed")
            {
                TempData["Error"] = "Completed sessions cannot be edited.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var vm = new SessionFormViewModel
            {
                SessionId = session.SessionId,
                CourseId = session.CourseId,
                InstructorId = session.InstructorId,
                ClassroomId = session.ClassroomId,
                StatusId = session.StatusId,
                SessionDate = session.SessionDate.Date,
                StartTime = session.StartTime.ToString("HH:mm"),
                EndTime = session.EndTime.ToString("HH:mm"),
                MaxCapacity = session.MaxCapacity
            };

            ViewBag.SessionStatus = session.Status?.Status ?? "Scheduled";
            return View(await PopulateDropdowns(vm));
        }

        // POST: Session/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> Edit(int id, SessionFormViewModel vm)
        {
            if (id != vm.SessionId)
                return BadRequest();

            var session = await _context.CourseSessions
                .Include(s => s.Status)
                .FirstOrDefaultAsync(s => s.SessionId == id);
            if (session == null)
                return NotFound();

            var currentStatus = session.Status?.Status ?? "Scheduled";

            // Completed: block all edits
            if (currentStatus == "Completed")
            {
                TempData["Error"] = "Completed sessions cannot be edited.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // Ongoing: only classroom change is allowed
            if (currentStatus == "Ongoing")
            {
                if (await HasClassroomConflict(vm.ClassroomId, session.SessionDate, session.StartTime, session.EndTime, excludeId: id))
                {
                    ViewBag.SessionStatus = currentStatus;
                    ModelState.AddModelError(string.Empty,
                        "This classroom is already booked during this session's time slot. Please choose a different classroom.");
                    return View(await PopulateDropdowns(vm));
                }

                session.ClassroomId = vm.ClassroomId;
                session.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                TempData["Success"] = "Classroom updated successfully.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // Scheduled / Cancelled: full edit allowed
            if (!ModelState.IsValid)
            {
                ViewBag.SessionStatus = currentStatus;
                return View(await PopulateDropdowns(vm));
            }

            if (!TryParseTimesToDateTimes(vm, out var startDt, out var endDt, out var timeError))
            {
                ViewBag.SessionStatus = currentStatus;
                ModelState.AddModelError(string.Empty, timeError);
                return View(await PopulateDropdowns(vm));
            }

            if (currentStatus == "Scheduled" && startDt <= DateTime.Now)
            {
                ViewBag.SessionStatus = currentStatus;
                ModelState.AddModelError(string.Empty,
                    "Session start time must be in the future. Please choose a later date or time.");
                return View(await PopulateDropdowns(vm));
            }

            if (await HasInstructorConflict(vm.InstructorId, vm.SessionDate, startDt, endDt, excludeId: id))
            {
                ViewBag.SessionStatus = currentStatus;
                ModelState.AddModelError(string.Empty,
                    "This instructor already has a session scheduled during the selected time slot. Please choose a different time or instructor.");
                return View(await PopulateDropdowns(vm));
            }

            if (await HasClassroomConflict(vm.ClassroomId, vm.SessionDate, startDt, endDt, excludeId: id))
            {
                ViewBag.SessionStatus = currentStatus;
                ModelState.AddModelError(string.Empty,
                    "This classroom is already booked during the selected time slot. Please choose a different classroom or time.");
                return View(await PopulateDropdowns(vm));
            }

            if (vm.MaxCapacity < session.CurrentEnrollment)
            {
                ViewBag.SessionStatus = currentStatus;
                ModelState.AddModelError(nameof(vm.MaxCapacity),
                    $"Max capacity cannot be less than the current enrollment count ({session.CurrentEnrollment}).");
                return View(await PopulateDropdowns(vm));
            }

            session.CourseId = vm.CourseId;
            session.InstructorId = vm.InstructorId;
            session.ClassroomId = vm.ClassroomId;
            session.StatusId = vm.StatusId;
            session.SessionDate = vm.SessionDate.Date;
            session.StartTime = startDt;
            session.EndTime = endDt;
            session.MaxCapacity = vm.MaxCapacity;
            session.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Session updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Session/Delete/5
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> Delete(int id)
        {
            var session = await _context.CourseSessions
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .Include(s => s.Classroom)
                .Include(s => s.Status)
                .Include(s => s.Enrollments)
                .FirstOrDefaultAsync(s => s.SessionId == id);

            if (session == null)
                return NotFound();

            var instructorUser = await _context.Users.FindAsync(session.Instructor.UserId);
            ViewBag.InstructorName = instructorUser?.UserName ?? $"Instructor {session.InstructorId}";

            return View(session);
        }

        // POST: Session/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var session = await _context.CourseSessions
                .Include(s => s.Enrollments)
                .FirstOrDefaultAsync(s => s.SessionId == id);

            if (session == null)
                return NotFound();

            if (session.Enrollments.Any())
            {
                TempData["Error"] = "Cannot delete a session that has active enrollments. Cancel the session instead.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            _context.CourseSessions.Remove(session);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Session deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private async Task<SessionFormViewModel> PopulateDropdowns(SessionFormViewModel vm)
        {
            vm.Courses = await _context.Courses
                .Where(c => c.IsActive)
                .OrderBy(c => c.Title)
                .Select(c => new SelectListItem
                {
                    Value = c.CourseId.ToString(),
                    Text = $"{c.CourseCode} – {c.Title}"
                })
                .ToListAsync();

            var dropdownInstructors = await _context.Instructors.ToListAsync();
            var dropdownUserIds = dropdownInstructors.Select(i => i.UserId).ToList();
            var dropdownUsersAll = await _context.Users.ToListAsync();
            var dropdownUsers = dropdownUsersAll
                .Where(u => dropdownUserIds.Contains(u.Id))
                .ToDictionary(u => u.Id, u => u.UserName ?? $"User {u.Id}");
            vm.Instructors = dropdownInstructors
                .Select(i => new SelectListItem
                {
                    Value = i.InstructorId.ToString(),
                    Text = dropdownUsers.TryGetValue(i.UserId, out var n) ? n : $"Instructor {i.InstructorId}"
                })
                .OrderBy(x => x.Text)
                .ToList();

            vm.Classrooms = await _context.Classrooms
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.ClassroomId.ToString(),
                    Text = $"{c.Name} – {c.Building}, Floor {c.Floor} (capacity {c.Capacity})"
                })
                .ToListAsync();

            vm.Statuses = await _context.CourseSessionStatuses
                .Select(s => new SelectListItem
                {
                    Value = s.StatusId.ToString(),
                    Text = s.Status
                })
                .ToListAsync();

            return vm;
        }

        private static bool TryParseTimesToDateTimes(
            SessionFormViewModel vm,
            out DateTime startDt,
            out DateTime endDt,
            out string error)
        {
            startDt = default;
            endDt = default;
            error = string.Empty;

            if (!TimeSpan.TryParse(vm.StartTime, out var startTs))
            {
                error = "Start time is invalid. Please use HH:MM format.";
                return false;
            }
            if (!TimeSpan.TryParse(vm.EndTime, out var endTs))
            {
                error = "End time is invalid. Please use HH:MM format.";
                return false;
            }
            if (endTs <= startTs)
            {
                error = "End time must be after start time.";
                return false;
            }

            startDt = vm.SessionDate.Date + startTs;
            endDt = vm.SessionDate.Date + endTs;
            return true;
        }

        private async Task<bool> HasInstructorConflict(
            int instructorId, DateTime date, DateTime start, DateTime end, int? excludeId = null)
        {
            var query = _context.CourseSessions.Where(s =>
                s.InstructorId == instructorId &&
                s.Status.Status != "Cancelled" &&
                s.SessionDate.Date == date.Date &&
                s.StartTime < end &&
                s.EndTime > start);

            if (excludeId.HasValue)
                query = query.Where(s => s.SessionId != excludeId.Value);

            return await query.AnyAsync();
        }

        private async Task<bool> HasClassroomConflict(
            int classroomId, DateTime date, DateTime start, DateTime end, int? excludeId = null)
        {
            var query = _context.CourseSessions.Where(s =>
                s.ClassroomId == classroomId &&
                s.Status.Status != "Cancelled" &&
                s.SessionDate.Date == date.Date &&
                s.StartTime < end &&
                s.EndTime > start);

            if (excludeId.HasValue)
                query = query.Where(s => s.SessionId != excludeId.Value);

            return await query.AnyAsync();
        }
    }
}
