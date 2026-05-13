using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Frontend.Models;
using Web_API.Models;

namespace MVC_Frontend.Controllers
{
    [Authorize(Roles = "Training Coordinator")]
    public class CategoryController : Controller
    {
        private readonly TrainingInstituteDBContext _context;

        public CategoryController(TrainingInstituteDBContext context)
        {
            _context = context;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.Courses)
                .OrderBy(c => c.Name)
                .ToListAsync();
            return View(categories);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            ViewBag.ParentCategories = new SelectList(
                _context.Categories.OrderBy(c => c.Name), "CategoryId", "Name");
            return View(new CategoryFormViewModel());
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ParentCategories = new SelectList(
                    _context.Categories.OrderBy(c => c.Name), "CategoryId", "Name");
                return View(vm);
            }

            if (await _context.Categories.AnyAsync(c => c.Name == vm.Name.Trim()))
            {
                ModelState.AddModelError(nameof(vm.Name), "A category with this name already exists.");
                ViewBag.ParentCategories = new SelectList(
                    _context.Categories.OrderBy(c => c.Name), "CategoryId", "Name");
                return View(vm);
            }

            _context.Categories.Add(new Category
            {
                Name = vm.Name.Trim(),
                Description = vm.Description?.Trim(),
                ParentCategoryId = vm.ParentCategoryId
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Category \"{vm.Name.Trim()}\" created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            ViewBag.ParentCategories = new SelectList(
                _context.Categories.Where(c => c.CategoryId != id).OrderBy(c => c.Name),
                "CategoryId", "Name", category.ParentCategoryId);

            return View(new CategoryFormViewModel
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                ParentCategoryId = category.ParentCategoryId
            });
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryFormViewModel vm)
        {
            if (id != vm.CategoryId)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.ParentCategories = new SelectList(
                    _context.Categories.Where(c => c.CategoryId != id).OrderBy(c => c.Name),
                    "CategoryId", "Name");
                return View(vm);
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            category.Name = vm.Name.Trim();
            category.Description = vm.Description?.Trim();
            category.ParentCategoryId = vm.ParentCategoryId;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Category \"{category.Name}\" updated.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Category/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            if (await _context.Courses.AnyAsync(c => c.CategoryId == id))
            {
                TempData["Error"] = "Cannot delete a category that has courses assigned to it.";
                return RedirectToAction(nameof(Index));
            }

            if (await _context.Categories.AnyAsync(c => c.ParentCategoryId == id))
            {
                TempData["Error"] = "Cannot delete a category that has sub-categories.";
                return RedirectToAction(nameof(Index));
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Category \"{category.Name}\" deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
