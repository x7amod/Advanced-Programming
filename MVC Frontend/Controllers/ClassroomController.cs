using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Frontend.Models;
using Web_API.Models;

namespace MVC_Frontend.Controllers;

[Authorize(Roles = AppRoles.Coordinator)]
public class ClassroomController : Controller
{
    private readonly TrainingInstituteDBContext _context;

    public ClassroomController(TrainingInstituteDBContext context)
    {
        _context = context;
    }

    // ── Index ─────────────────────────────────────────────────────────────────
    public async Task<IActionResult> Index()
    {
        var classrooms = await _context.Classrooms
            .Include(c => c.ClassroomEquipments)
            .OrderBy(c => c.Building).ThenBy(c => c.Name)
            .ToListAsync();

        return View(classrooms);
    }

    // ── Details (with equipment) ──────────────────────────────────────────────
    public async Task<IActionResult> Details(int id)
    {
        var classroom = await _context.Classrooms
            .Include(c => c.ClassroomEquipments)
            .FirstOrDefaultAsync(c => c.ClassroomId == id);

        if (classroom == null) return NotFound();
        return View(classroom);
    }

    // ── Create GET ────────────────────────────────────────────────────────────
    public IActionResult Create() => View(new ClassroomFormViewModel());

    // ── Create POST ───────────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClassroomFormViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        _context.Classrooms.Add(new Classroom
        {
            Name = vm.Name.Trim(),
            Location = vm.Location.Trim(),
            Building = vm.Building.Trim(),
            Floor = vm.Floor.Trim(),
            Capacity = vm.Capacity,
            IsActive = vm.IsActive
        });
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Classroom \"{vm.Name}\" created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Edit GET ──────────────────────────────────────────────────────────────
    public async Task<IActionResult> Edit(int id)
    {
        var c = await _context.Classrooms.FindAsync(id);
        if (c == null) return NotFound();

        return View(new ClassroomFormViewModel
        {
            ClassroomId = c.ClassroomId,
            Name = c.Name,
            Location = c.Location,
            Building = c.Building,
            Floor = c.Floor,
            Capacity = c.Capacity,
            IsActive = c.IsActive
        });
    }

    // ── Edit POST ─────────────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ClassroomFormViewModel vm)
    {
        if (id != vm.ClassroomId) return BadRequest();
        if (!ModelState.IsValid) return View(vm);

        var classroom = await _context.Classrooms.FindAsync(id);
        if (classroom == null) return NotFound();

        // Warn if reducing capacity below current active sessions
        var maxEnrollment = await _context.CourseSessions
            .Where(s => s.ClassroomId == id && s.Status.Status != "Completed" && s.Status.Status != "Cancelled")
            .Select(s => (int?)s.MaxCapacity)
            .MaxAsync();

        if (maxEnrollment.HasValue && vm.Capacity < maxEnrollment.Value)
        {
            ModelState.AddModelError(nameof(vm.Capacity),
                $"Capacity cannot be reduced below {maxEnrollment.Value} — an active session uses this classroom with that capacity.");
            return View(vm);
        }

        classroom.Name = vm.Name.Trim();
        classroom.Location = vm.Location.Trim();
        classroom.Building = vm.Building.Trim();
        classroom.Floor = vm.Floor.Trim();
        classroom.Capacity = vm.Capacity;
        classroom.IsActive = vm.IsActive;

        await _context.SaveChangesAsync();
        TempData["Success"] = $"Classroom \"{classroom.Name}\" updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // ── Deactivate POST ───────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(int id)
    {
        var classroom = await _context.Classrooms.FindAsync(id);
        if (classroom == null) return NotFound();

        var hasActiveSessions = await _context.CourseSessions
            .AnyAsync(s => s.ClassroomId == id &&
                           s.Status.Status != "Completed" &&
                           s.Status.Status != "Cancelled");

        if (hasActiveSessions)
        {
            TempData["Error"] = "Cannot deactivate a classroom that has active or scheduled sessions.";
            return RedirectToAction(nameof(Details), new { id });
        }

        classroom.IsActive = !classroom.IsActive;
        await _context.SaveChangesAsync();

        TempData["Success"] = classroom.IsActive
            ? $"Classroom \"{classroom.Name}\" reactivated."
            : $"Classroom \"{classroom.Name}\" deactivated.";
        return RedirectToAction(nameof(Index));
    }

    // ── Add Equipment POST ────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddEquipment(ClassroomEquipmentFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please fill in all equipment fields correctly.";
            return RedirectToAction(nameof(Details), new { id = vm.ClassroomId });
        }

        var classroom = await _context.Classrooms.FindAsync(vm.ClassroomId);
        if (classroom == null) return NotFound();

        // EquipmentId is an IDENTITY column — let the database generate it
        _context.ClassroomEquipments.Add(new ClassroomEquipment
        {
            ClassroomId = vm.ClassroomId,
            EquipmentType = vm.EquipmentType.Trim(),
            Quantity = vm.Quantity,
            Description = string.IsNullOrWhiteSpace(vm.Description) ? null : vm.Description.Trim()
        });
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Equipment \"{vm.EquipmentType}\" added.";
        return RedirectToAction(nameof(Details), new { id = vm.ClassroomId });
    }

    // ── Remove Equipment POST ─────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveEquipment(int equipmentId, int classroomId)
    {
        var equipment = await _context.ClassroomEquipments
            .FirstOrDefaultAsync(e => e.EquipmentId == equipmentId && e.ClassroomId == classroomId);

        if (equipment != null)
        {
            _context.ClassroomEquipments.Remove(equipment);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Equipment removed.";
        }

        return RedirectToAction(nameof(Details), new { id = classroomId });
    }
}
