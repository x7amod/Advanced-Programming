using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Frontend.Models;
using Web_API.Models;

namespace MVC_Frontend.Controllers
{
    [Authorize(Roles = "Coordinator")]
    public class SubjectAreaController : Controller
    {
        private readonly TrainingInstituteDBContext _context;

        public SubjectAreaController(TrainingInstituteDBContext context)
        {
            _context = context;
        }

        // GET: SubjectArea
        public async Task<IActionResult> Index()
        {
            var areas = await _context.SubjectAreas
                .Include(s => s.Courses)
                .OrderBy(s => s.Name)
                .ToListAsync();
            return View(areas);
        }

        // GET: SubjectArea/Create
        public IActionResult Create()
        {
            return View(new SubjectAreaFormViewModel());
        }

        // POST: SubjectArea/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubjectAreaFormViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            if (await _context.SubjectAreas.AnyAsync(s => s.Name == vm.Name.Trim()))
            {
                ModelState.AddModelError(nameof(vm.Name), "A subject area with this name already exists.");
                return View(vm);
            }

            _context.SubjectAreas.Add(new SubjectArea
            {
                Name = vm.Name.Trim(),
                Description = vm.Description?.Trim()
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Subject area \"{vm.Name.Trim()}\" created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: SubjectArea/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var area = await _context.SubjectAreas.FindAsync(id);
            if (area == null)
                return NotFound();

            return View(new SubjectAreaFormViewModel
            {
                SubjectAreaId = area.SubjectAreaId,
                Name = area.Name,
                Description = area.Description
            });
        }

        // POST: SubjectArea/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubjectAreaFormViewModel vm)
        {
            if (id != vm.SubjectAreaId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(vm);

            var area = await _context.SubjectAreas.FindAsync(id);
            if (area == null)
                return NotFound();

            area.Name = vm.Name.Trim();
            area.Description = vm.Description?.Trim();
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Subject area \"{area.Name}\" updated.";
            return RedirectToAction(nameof(Index));
        }

        // POST: SubjectArea/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var area = await _context.SubjectAreas.FindAsync(id);
            if (area == null)
                return NotFound();

            if (await _context.Courses.AnyAsync(c => c.SubjectAreaId == id))
            {
                TempData["Error"] = "Cannot delete a subject area that has courses assigned to it.";
                return RedirectToAction(nameof(Index));
            }

            _context.SubjectAreas.Remove(area);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Subject area \"{area.Name}\" deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
