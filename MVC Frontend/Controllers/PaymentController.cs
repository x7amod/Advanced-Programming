using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Frontend.Helpers;
using MVC_Frontend.Models;
using Web_API.Models;

namespace MVC_Frontend.Controllers;

[Authorize]
public class PaymentController : Controller
{
    private readonly TrainingInstituteDBContext _context;

    public PaymentController(TrainingInstituteDBContext context)
    {
        _context = context;
    }

    // ── 1. All Payment Records (Coordinator) ──────────────────────────────────
    [Authorize(Roles = AppRoles.Coordinator)]
    public async Task<IActionResult> Index(string? filterStatus)
    {
        var today = DateTime.Today;

        var rows = await _context.PaymentRecords
            .Where(p => string.IsNullOrWhiteSpace(filterStatus) || p.Status.Status == filterStatus)
            .OrderByDescending(p => p.DueDate)
            .Select(p => new PaymentRecordRowViewModel
            {
                PaymentRecordId  = p.PaymentRecordId,
                TraineeName      = p.Enrollment.Trainee.User.UserName ?? $"Trainee {p.Enrollment.TraineeId}",
                CourseTitle      = p.Enrollment.Session.Course.Title,
                SessionDate      = p.Enrollment.Session.SessionDate,
                TotalAmount      = p.TotalAmount,
                PaidAmount       = p.PaymentTransactions.Sum(t => t.Amount),
                Status           = p.Status.Status,
                DueDate          = p.DueDate,
                IsOverdue        = p.Status.Status != "Paid" && p.DueDate < today
            })
            .ToListAsync();

        var statuses = await _context.PaymentStatuses
            .Select(s => new SelectListItem { Value = s.Status, Text = s.Status })
            .ToListAsync();

        return View(new PaymentIndexViewModel
        {
            Records        = rows,
            FilterStatus   = filterStatus,
            Statuses       = statuses,
            TotalOutstanding = rows.Sum(r => r.OutstandingAmount),
            TotalCollected   = rows.Sum(r => r.PaidAmount)
        });
    }

    // ── 2. Payment Record Details (Coordinator) ───────────────────────────────
    [Authorize(Roles = AppRoles.Coordinator)]
    public async Task<IActionResult> Details(int id)
    {
        var today = DateTime.Today;

        var vm = await _context.PaymentRecords
            .Where(p => p.PaymentRecordId == id)
            .Select(p => new PaymentDetailsViewModel
            {
                PaymentRecordId = p.PaymentRecordId,
                TraineeName     = p.Enrollment.Trainee.User.UserName ?? $"Trainee {p.Enrollment.TraineeId}",
                CourseTitle     = p.Enrollment.Session.Course.Title,
                SessionDate     = p.Enrollment.Session.SessionDate,
                TotalAmount     = p.TotalAmount,
                PaidAmount      = p.PaymentTransactions.Sum(t => t.Amount),
                Status          = p.Status.Status,
                DueDate         = p.DueDate,
                IssuedDate      = p.IssuedDate,
                IsOverdue       = p.Status.Status != "Paid" && p.DueDate < today,
                Transactions    = p.PaymentTransactions
                    .OrderByDescending(t => t.PaymentDate)
                    .Select(t => new PaymentTransactionRowViewModel
                    {
                        TransactionId  = t.TransactionId,
                        Amount         = t.Amount,
                        PaymentMethod  = t.PaymentMethodNavigation.PaymentMethod1,
                        PaymentDate    = t.PaymentDate,
                        Notes          = t.Notes
                    }).ToList()
            })
            .FirstOrDefaultAsync();

        if (vm == null) return NotFound();
        return View(vm);
    }

    // ── 3. Create Payment Record GET (Coordinator) ────────────────────────────
    [Authorize(Roles = AppRoles.Coordinator)]
    public async Task<IActionResult> Create(int enrollmentId)
    {
        var existing = await _context.PaymentRecords.AnyAsync(p => p.EnrollmentId == enrollmentId);
        if (existing)
        {
            TempData["Error"] = "A payment record already exists for this enrollment.";
            return RedirectToAction(nameof(Index));
        }

        var data = await _context.Enrollments
            .Where(e => e.EnrollmentId == enrollmentId)
            .Select(e => new
            {
                StatusName    = e.EnrollmentStatus.Status,
                TraineeName   = e.Trainee.User.UserName ?? $"Trainee {e.TraineeId}",
                CourseTitle   = e.Session.Course.Title,
                SessionDate   = e.Session.SessionDate,
                EnrollmentFee = e.Session.Course.EnrollmentFee
            })
            .FirstOrDefaultAsync();

        if (data == null) return NotFound();

        if (data.StatusName == "Dropped")
        {
            TempData["Error"] = "Cannot create an invoice for a dropped enrollment.";
            return RedirectToAction(nameof(Index));
        }

        return View(new CreatePaymentRecordViewModel
        {
            EnrollmentId = enrollmentId,
            TraineeName  = data.TraineeName,
            CourseTitle  = data.CourseTitle,
            SessionDate  = data.SessionDate,
            TotalAmount  = data.EnrollmentFee,
            DueDate      = DateTime.Today.AddDays(14)
        });
    }

    // ── 3. Create Payment Record POST (Coordinator) ───────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.Coordinator)]
    public async Task<IActionResult> Create(CreatePaymentRecordViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var existing = await _context.PaymentRecords.AnyAsync(p => p.EnrollmentId == vm.EnrollmentId);
        if (existing)
        {
            TempData["Error"] = "A payment record already exists for this enrollment.";
            return RedirectToAction(nameof(Index));
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var coordinator = await _context.Coordinators.FirstOrDefaultAsync(c => c.UserId == userId);
        if (coordinator == null) return Forbid();

        var unpaidStatus = await _context.PaymentStatuses.FirstOrDefaultAsync(s => s.Status == "Unpaid");
        if (unpaidStatus == null) { TempData["Error"] = "Payment status configuration error."; return View(vm); }

        var now = DateTime.Now;
        var record = new PaymentRecord
        {
            EnrollmentId  = vm.EnrollmentId,
            CoordinatorId = coordinator.CoordinatorId,
            StatusId      = unpaidStatus.StatusId,
            TotalAmount   = vm.TotalAmount,
            DueDate       = vm.DueDate,
            IssuedDate    = now,
            CreatedAt     = now,
            UpdatedAt     = now
        };
        _context.PaymentRecords.Add(record);
        await _context.SaveChangesAsync();

        // Notify the trainee
        var traineeInfo = await _context.Enrollments
            .Where(e => e.EnrollmentId == vm.EnrollmentId)
            .Select(e => new { e.Trainee.UserId, CourseTitle = e.Session.Course.Title })
            .FirstOrDefaultAsync();

        if (traineeInfo != null)
        {
            await NotificationHelper.CreateAsync(_context, traineeInfo.UserId,
                "Payment Invoice Created",
                $"An invoice of {vm.TotalAmount:C} has been issued for {traineeInfo.CourseTitle}. Due by {vm.DueDate:MMM dd, yyyy}.",
                "Payment", "PaymentRecord");
        }

        TempData["Success"] = "Payment record created successfully.";
        return RedirectToAction(nameof(Details), new { id = record.PaymentRecordId });
    }

    // ── 4. Record Transaction GET (Coordinator) ───────────────────────────────
    [Authorize(Roles = AppRoles.Coordinator)]
    public async Task<IActionResult> RecordTransaction(int paymentRecordId)
    {
        var data = await _context.PaymentRecords
            .Where(p => p.PaymentRecordId == paymentRecordId)
            .Select(p => new
            {
                p.TotalAmount,
                TraineeName = p.Enrollment.Trainee.User.UserName ?? $"Trainee {p.Enrollment.TraineeId}",
                CourseTitle = p.Enrollment.Session.Course.Title,
                PaidSoFar   = p.PaymentTransactions.Sum(t => t.Amount)
            })
            .FirstOrDefaultAsync();

        if (data == null) return NotFound();

        var methods = await _context.PaymentMethods
            .Select(m => new SelectListItem { Value = m.PaymentMethodId.ToString(), Text = m.PaymentMethod1 })
            .ToListAsync();

        return View(new RecordTransactionViewModel
        {
            PaymentRecordId = paymentRecordId,
            TraineeName     = data.TraineeName,
            CourseTitle     = data.CourseTitle,
            TotalAmount     = data.TotalAmount,
            PaidSoFar       = data.PaidSoFar,
            PaymentMethods  = methods
        });
    }

    // ── 4. Record Transaction POST (Coordinator) ──────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.Coordinator)]
    public async Task<IActionResult> RecordTransaction(RecordTransactionViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.PaymentMethods = await _context.PaymentMethods
                .Select(m => new SelectListItem { Value = m.PaymentMethodId.ToString(), Text = m.PaymentMethod1 })
                .ToListAsync();
            return View(vm);
        }

        // Fetch only what we need to save — no collection includes
        var record = await _context.PaymentRecords.FindAsync(vm.PaymentRecordId);
        if (record == null) return NotFound();

        var paidSoFar = await _context.PaymentTransactions
            .Where(t => t.PaymentRecordId == vm.PaymentRecordId)
            .SumAsync(t => t.Amount);

        var outstanding = record.TotalAmount - paidSoFar;
        if (vm.Amount > outstanding)
        {
            ModelState.AddModelError(nameof(vm.Amount), $"Amount cannot exceed the outstanding balance of {outstanding:C}.");
            vm.PaymentMethods = await _context.PaymentMethods
                .Select(m => new SelectListItem { Value = m.PaymentMethodId.ToString(), Text = m.PaymentMethod1 })
                .ToListAsync();
            vm.TotalAmount = record.TotalAmount;
            vm.PaidSoFar   = paidSoFar;
            return View(vm);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var coordinator = await _context.Coordinators.FirstOrDefaultAsync(c => c.UserId == userId);
        if (coordinator == null) return Forbid();

        var method = await _context.PaymentMethods.FindAsync(vm.PaymentMethodId);
        if (method == null)
        {
            ModelState.AddModelError("PaymentMethodId", "Invalid payment method.");
            vm.PaymentMethods = await _context.PaymentMethods
                .Select(m => new SelectListItem { Value = m.PaymentMethodId.ToString(), Text = m.PaymentMethod1 })
                .ToListAsync();
            return View(vm);
        }

        var now       = DateTime.Now;
        var totalPaid = paidSoFar + vm.Amount;

        _context.PaymentTransactions.Add(new PaymentTransaction
        {
            PaymentRecordId = vm.PaymentRecordId,
            CoordinatorId   = coordinator.CoordinatorId,
            PaymentMethodId = vm.PaymentMethodId,
            PaymentMethod   = method.PaymentMethod1,
            Amount          = vm.Amount,
            PaymentDate     = now,
            Notes           = string.IsNullOrWhiteSpace(vm.Notes) ? null : vm.Notes.Trim(),
            CreatedAt       = now
        });

        // Update status
        var newStatusName = totalPaid >= record.TotalAmount ? "Paid"
                          : totalPaid > 0                   ? "Partial"
                          : "Unpaid";
        var newStatus = await _context.PaymentStatuses.FirstOrDefaultAsync(s => s.Status == newStatusName);
        if (newStatus != null) { record.StatusId = newStatus.StatusId; record.UpdatedAt = now; }

        await _context.SaveChangesAsync();

        // Notify trainee
        var traineeInfo = await _context.Enrollments
            .Where(e => e.EnrollmentId == record.EnrollmentId)
            .Select(e => new { e.Trainee.UserId, CourseTitle = e.Session.Course.Title })
            .FirstOrDefaultAsync();

        if (traineeInfo != null)
        {
            var remaining = record.TotalAmount - totalPaid;
            var msg = totalPaid >= record.TotalAmount
                ? $"Your payment of {vm.Amount:C} for {traineeInfo.CourseTitle} has been received. Your balance is fully settled."
                : $"Your payment of {vm.Amount:C} for {traineeInfo.CourseTitle} has been received. Remaining balance: {remaining:C}.";

            await NotificationHelper.CreateAsync(_context, traineeInfo.UserId,
                "Payment Received", msg, "Payment", "PaymentRecord");
        }

        TempData["Success"] = $"Payment of {vm.Amount:C} recorded successfully.";
        return RedirectToAction(nameof(Details), new { id = vm.PaymentRecordId });
    }

    // ── 5. My Payments (Trainee) ──────────────────────────────────────────────
    [Authorize(Roles = AppRoles.Trainee)]
    public async Task<IActionResult> MyPayments()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.UserId == userId);
        if (trainee == null)
        {
            TempData["Error"] = "Trainee profile not found.";
            return View(new MyPaymentsViewModel());
        }

        var today = DateTime.Today;

        var rows = await _context.PaymentRecords
            .Where(p => p.Enrollment.TraineeId == trainee.TraineeId)
            .OrderByDescending(p => p.DueDate)
            .Select(p => new PaymentRecordRowViewModel
            {
                PaymentRecordId = p.PaymentRecordId,
                TraineeName     = p.Enrollment.Trainee.User.UserName ?? $"Trainee {p.Enrollment.TraineeId}",
                CourseTitle     = p.Enrollment.Session.Course.Title,
                SessionDate     = p.Enrollment.Session.SessionDate,
                TotalAmount     = p.TotalAmount,
                PaidAmount      = p.PaymentTransactions.Sum(t => t.Amount),
                Status          = p.Status.Status,
                DueDate         = p.DueDate,
                IsOverdue       = p.Status.Status != "Paid" && p.DueDate < today
            })
            .ToListAsync();

        return View(new MyPaymentsViewModel
        {
            Records    = rows,
            TotalOwed  = rows.Sum(r => r.TotalAmount),
            TotalPaid  = rows.Sum(r => r.PaidAmount)
        });
    }
}
