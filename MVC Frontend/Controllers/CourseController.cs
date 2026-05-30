using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Frontend.Models;
using Web_API.Models;

namespace MVC_Frontend.Controllers
{
    public class CourseController : Controller
    {
        private readonly TrainingInstituteDBContext _context;

        public CourseController(TrainingInstituteDBContext context)
        {
            _context = context;
        }

        // GET: Course — browse course catalog with optional search/filter
        public async Task<IActionResult> Index(string? search, int? categoryId, int? subjectAreaId, bool showInactive = false)
        {
            var query = _context.Courses
                .Include(c => c.Category)
                .Include(c => c.SubjectArea)
                .Include(c => c.PrerequisiteCourse)
                .AsQueryable();

            if (!showInactive)
                query = query.Where(c => c.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c =>
                    c.Title.Contains(search) ||
                    c.CourseCode.Contains(search) ||
                    (c.Description != null && c.Description.Contains(search)));

            if (categoryId.HasValue)
                query = query.Where(c => c.CategoryId == categoryId);

            if (subjectAreaId.HasValue)
                query = query.Where(c => c.SubjectAreaId == subjectAreaId);

            ViewBag.Categories = new SelectList(
                await _context.Categories.OrderBy(c => c.Name).ToListAsync(),
                "CategoryId", "Name", categoryId);
            ViewBag.SubjectAreas = new SelectList(
                await _context.SubjectAreas.OrderBy(s => s.Name).ToListAsync(),
                "SubjectAreaId", "Name", subjectAreaId);
            ViewBag.Search = search;
            ViewBag.ShowInactive = showInactive;

            return View(await query.OrderBy(c => c.Title).ToListAsync());
        }

        // GET: Course/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.SubjectArea)
                .Include(c => c.PrerequisiteCourse)
                .Include(c => c.CourseSessions.Where(s => s.SessionDate >= DateTime.Now))
                    .ThenInclude(s => s.Instructor)
                .Include(c => c.CourseSessions)
                    .ThenInclude(s => s.Status)
                .Include(c => c.CertificationRequiredCourses)
                    .ThenInclude(crc => crc.CertificationTrack)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
                return NotFound();

            return View(course);
        }

        // GET: Course/Create
        [Authorize(Roles = AppRoles.Coordinator)]
        public IActionResult Create()
        {
            return View(PopulateDropdowns(new CourseFormViewModel { IsActive = true }));
        }

        // POST: Course/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> Create(CourseFormViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(PopulateDropdowns(vm));

            if (await _context.Courses.AnyAsync(c => c.CourseCode == vm.CourseCode.Trim().ToUpper()))
            {
                ModelState.AddModelError(nameof(vm.CourseCode), "This course code is already in use.");
                return View(PopulateDropdowns(vm));
            }

            if (vm.PrerequisiteCourseId.HasValue)
            {
                var prereq = await _context.Courses.FindAsync(vm.PrerequisiteCourseId.Value);
                if (prereq?.PrerequisiteCourseId != null)
                {
                    ModelState.AddModelError(nameof(vm.PrerequisiteCourseId),
                        "This course already has a prerequisite. Only single-level prerequisites are allowed.");
                    return View(PopulateDropdowns(vm));
                }
            }

            var course = new Course
            {
                SubjectAreaId = vm.SubjectAreaId,
                CategoryId = vm.CategoryId,
                PrerequisiteCourseId = vm.PrerequisiteCourseId,
                CourseCode = vm.CourseCode.Trim().ToUpper(),
                Title = vm.Title.Trim(),
                Description = vm.Description?.Trim(),
                DurationHours = vm.DurationHours,
                MaxCapacity = vm.MaxCapacity,
                EnrollmentFee = vm.EnrollmentFee,
                EquipmentRequirements = vm.EquipmentRequirements?.Trim(),
                IsActive = vm.IsActive,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Course \"{course.Title}\" created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Course/Edit/5
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound();

            var vm = new CourseFormViewModel
            {
                CourseId = course.CourseId,
                SubjectAreaId = course.SubjectAreaId,
                CategoryId = course.CategoryId,
                PrerequisiteCourseId = course.PrerequisiteCourseId,
                CourseCode = course.CourseCode,
                Title = course.Title,
                Description = course.Description,
                DurationHours = course.DurationHours,
                MaxCapacity = course.MaxCapacity,
                EnrollmentFee = course.EnrollmentFee,
                EquipmentRequirements = course.EquipmentRequirements,
                IsActive = course.IsActive
            };

            return View(PopulateDropdowns(vm, excludeId: course.CourseId));
        }

        // POST: Course/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> Edit(int id, CourseFormViewModel vm)
        {
            if (id != vm.CourseId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(PopulateDropdowns(vm, excludeId: id));

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound();

            if (await _context.Courses.AnyAsync(c => c.CourseCode == vm.CourseCode.Trim().ToUpper() && c.CourseId != id))
            {
                ModelState.AddModelError(nameof(vm.CourseCode), "This course code is already in use.");
                return View(PopulateDropdowns(vm, excludeId: id));
            }

            if (vm.PrerequisiteCourseId.HasValue)
            {
                var prereq = await _context.Courses.FindAsync(vm.PrerequisiteCourseId.Value);
                if (prereq?.PrerequisiteCourseId != null)
                {
                    ModelState.AddModelError(nameof(vm.PrerequisiteCourseId),
                        "This course already has a prerequisite. Only single-level prerequisites are allowed.");
                    return View(PopulateDropdowns(vm, excludeId: id));
                }
                // Prevent circular: chosen prerequisite must not already list this course as its prerequisite
                if (prereq?.CourseId == id)
                {
                    ModelState.AddModelError(nameof(vm.PrerequisiteCourseId), "A course cannot be its own prerequisite.");
                    return View(PopulateDropdowns(vm, excludeId: id));
                }
            }

            course.SubjectAreaId = vm.SubjectAreaId;
            course.CategoryId = vm.CategoryId;
            course.PrerequisiteCourseId = vm.PrerequisiteCourseId;
            course.CourseCode = vm.CourseCode.Trim().ToUpper();
            course.Title = vm.Title.Trim();
            course.Description = vm.Description?.Trim();
            course.DurationHours = vm.DurationHours;
            course.MaxCapacity = vm.MaxCapacity;
            course.EnrollmentFee = vm.EnrollmentFee;
            course.EquipmentRequirements = vm.EquipmentRequirements?.Trim();
            course.IsActive = vm.IsActive;
            course.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Course \"{course.Title}\" updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Course/Deactivate/5 — confirmation page
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> Deactivate(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.SubjectArea)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
                return NotFound();

            return View(course);
        }

        // POST: Course/Deactivate/5
        [HttpPost, ActionName("Deactivate")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> DeactivateConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound();

            course.IsActive = false;
            course.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Course \"{course.Title}\" has been deactivated.";
            return RedirectToAction(nameof(Index));
        }

        private CourseFormViewModel PopulateDropdowns(CourseFormViewModel vm, int? excludeId = null)
        {
            vm.SubjectAreas = _context.SubjectAreas
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem { Value = s.SubjectAreaId.ToString(), Text = s.Name });

            vm.Categories = _context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.Name });

            // Only courses with no prerequisite themselves can be selected as a prerequisite (single-level rule)
            var prereqQuery = _context.Courses.Where(c => c.IsActive && c.PrerequisiteCourseId == null);
            if (excludeId.HasValue)
                prereqQuery = prereqQuery.Where(c => c.CourseId != excludeId.Value);

            vm.PrerequisiteCourses = prereqQuery
                .OrderBy(c => c.Title)
                .Select(c => new SelectListItem
                {
                    Value = c.CourseId.ToString(),
                    Text = $"{c.CourseCode} – {c.Title}"
                });

            return vm;
        }
    }
}
