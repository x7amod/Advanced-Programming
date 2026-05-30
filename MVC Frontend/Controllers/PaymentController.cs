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
        IQueryable<PaymentRecord> query = _context.PaymentRecords
            .Include(p => p.Enrollment).ThenInclude(e => e.Trainee)
            .Include(p => p.Enrollment).ThenInclude(e => e.Session).ThenInclude(s => s.Course)
            .Include(p => p.PaymentTransactions)
            .Include(p => p.Status);

        if (!string.IsNullOrWhiteSpace(filterStatus))
            query = query.Where(p => p.Status.Status == filterStatus);

        var records = await query
            .OrderByDescending(p => p.DueDate)
            .AsSplitQuery()
            .ToListAsync();

        var traineeUserIds = records.Select(p => p.Enrollment.Trainee.UserId).Distinct().ToList();
        var users = await _context.Users
            .Where(u => traineeUserIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.UserName ?? u.Id);

        var statuses = await _context.PaymentStatuses
            .Select(s => new SelectListItem { Value = s.Status, Text = s.Status })
            .ToListAsync();

        var rows = records.Select(p =>
        {
            var paid = p.PaymentTransactions.Sum(t => t.Amount);
            return new PaymentRecordRowViewModel
            {
                PaymentRecordId = p.PaymentRecordId,
                TraineeName = users.TryGetValue(p.Enrollment.Trainee.UserId, out var n)
                    ? n : $"Trainee {p.Enrollment.TraineeId}",
                CourseTitle = p.Enrollment.Session.Course.Title,
                SessionDate = p.Enrollment.Session.SessionDate,
                TotalAmount = p.TotalAmount,
                PaidAmount = paid,
                Status = p.Status.Status,
                DueDate = p.DueDate,
                IsOverdue = p.Status.Status != "Paid" && p.DueDate.Date < DateTime.Today
            };
        }).ToList();

        return View(new PaymentIndexViewModel
        {
            Records = rows,
            FilterStatus = filterStatus,
            Statuses = statuses,
            TotalOutstanding = rows.Sum(r => r.OutstandingAmount),
            TotalCollected = rows.Sum(r => r.PaidAmount)
        });
    }

    // ── 2. Payment Record Details (Coordinator) ───────────────────────────────
    [Authorize(Roles = AppRoles.Coordinator)]
    public async Task<IActionResult> Details(int id)
    {
        var record = await _context.PaymentRecords
            .Include(p => p.Enrollment).ThenInclude(e => e.Trainee)
            .Include(p => p.Enrollment).ThenInclude(e => e.Session).ThenInclude(s => s.Course)
            .Include(p => p.PaymentTransactions).ThenInclude(t => t.PaymentMethodNavigation)
            .Include(p => p.Status)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.PaymentRecordId == id);

        if (record == null) return NotFound();

        var traineeUser = await _context.Users.FindAsync(record.Enrollment.Trainee.UserId);
        var paid = record.PaymentTransactions.Sum(t => t.Amount);

        return View(new PaymentDetailsViewModel
        {
            PaymentRecordId = record.PaymentRecordId,
            TraineeName = traineeUser?.UserName ?? $"Trainee {record.Enrollment.TraineeId}",
            CourseTitle = record.Enrollment.Session.Course.Title,
            SessionDate = record.Enrollment.Session.SessionDate,
            TotalAmount = record.TotalAmount,
            PaidAmount = paid,
            Status = record.Status.Status,
            DueDate = record.DueDate,
            IssuedDate = record.IssuedDate,
            IsOverdue = record.Status.Status != "Paid" && record.DueDate.Date < DateTime.Today,
            Transactions = record.PaymentTransactions
                .OrderByDescending(t => t.PaymentDate)
                .Select(t => new PaymentTransactionRowViewModel
                {
                    TransactionId = t.TransactionId,
                    Amount = t.Amount,
                    PaymentMethod = t.PaymentMethodNavigation.PaymentMethod1,
                    PaymentDate = t.PaymentDate,
                    Notes = t.Notes
                }).ToList()
        });
    }

    // ── 3. Create Payment Record (Coordinator) ────────────────────────────────
    [Authorize(Roles = AppRoles.Coordinator)]
    public async Task<IActionResult> Create(int enrollmentId)
    {
        var existing = await _context.PaymentRecords
            .FirstOrDefaultAsync(p => p.EnrollmentId == enrollmentId);
        if (existing != null)
        {
            TempData["Error"] = "A payment record already exists for this enrollment.";
            return RedirectToAction(nameof(Index));
        }

        var enrollment = await _context.Enrollments
            .Include(e => e.Trainee)
            .Include(e => e.EnrollmentStatus)
            .Include(e => e.Session).ThenInclude(s => s.Course)
            .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);

        if (enrollment == null) return NotFound();

        if (enrollment.EnrollmentStatus.Status == "Dropped")
        {
            TempData["Error"] = "Cannot create an invoice for a dropped enrollment.";
            return RedirectToAction(nameof(Index));
        }

        var traineeUser = await _context.Users.FindAsync(enrollment.Trainee.UserId);

        return View(new CreatePaymentRecordViewModel
        {
            EnrollmentId = enrollmentId,
            TraineeName = traineeUser?.UserName ?? $"Trainee {enrollment.TraineeId}",
            CourseTitle = enrollment.Session.Course.Title,
            SessionDate = enrollment.Session.SessionDate,
            TotalAmount = enrollment.Session.Course.EnrollmentFee,
            DueDate = DateTime.Today.AddDays(14)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.Coordinator)]
    public async Task<IActionResult> Create(CreatePaymentRecordViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var existing = await _context.PaymentRecords
            .FirstOrDefaultAsync(p => p.EnrollmentId == vm.EnrollmentId);
        if (existing != null)
        {
            TempData["Error"] = "A payment record already exists for this enrollment.";
            return RedirectToAction(nameof(Index));
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var coordinator = await _context.Coordinators.FirstOrDefaultAsync(c => c.UserId == userId);
        if (coordinator == null) return Forbid();

        var unpaidStatus = await _context.PaymentStatuses.FirstOrDefaultAsync(s => s.Status == "Unpaid");
        if (unpaidStatus == null)
        {
            TempData["Error"] = "Payment status configuration error.";
            return View(vm);
        }

        var now = DateTime.Now;
        var record = new PaymentRecord
        {
            EnrollmentId = vm.EnrollmentId,
            CoordinatorId = coordinator.CoordinatorId,
            StatusId = unpaidStatus.StatusId,
            TotalAmount = vm.TotalAmount,
            DueDate = vm.DueDate,
            IssuedDate = now,
            CreatedAt = now,
            UpdatedAt = now
        };
        _context.PaymentRecords.Add(record);
        await _context.SaveChangesAsync();

        // Notify the trainee
        var enrollment = await _context.Enrollments
            .Include(e => e.Trainee)
            .Include(e => e.Session).ThenInclude(s => s.Course)
            .FirstOrDefaultAsync(e => e.EnrollmentId == vm.EnrollmentId);

        if (enrollment != null)
        {
            await NotificationHelper.CreateAsync(_context, enrollment.Trainee.UserId,
                "Payment Invoice Created",
                $"An invoice of {vm.TotalAmount:C} has been issued for {enrollment.Session.Course.Title}. Due by {vm.DueDate:MMM dd, yyyy}.",
                "Payment", "PaymentRecord");
        }

        TempData["Success"] = "Payment record created successfully.";
        return RedirectToAction(nameof(Details), new { id = record.PaymentRecordId });
    }

    // ── 4. Record a Payment Transaction (Coordinator) ─────────────────────────
    [Authorize(Roles = AppRoles.Coordinator)]
    public async Task<IActionResult> RecordTransaction(int paymentRecordId)
    {
        var record = await _context.PaymentRecords
            .Include(p => p.Enrollment).ThenInclude(e => e.Trainee)
            .Include(p => p.Enrollment).ThenInclude(e => e.Session).ThenInclude(s => s.Course)
            .Include(p => p.PaymentTransactions)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.PaymentRecordId == paymentRecordId);

        if (record == null) return NotFound();

        var traineeUser = await _context.Users.FindAsync(record.Enrollment.Trainee.UserId);
        var paid = record.PaymentTransactions.Sum(t => t.Amount);

        var methods = await _context.PaymentMethods
            .Select(m => new SelectListItem
            {
                Value = m.PaymentMethodId.ToString(),
                Text = m.PaymentMethod1
            })
            .ToListAsync();

        return View(new RecordTransactionViewModel
        {
            PaymentRecordId = paymentRecordId,
            TraineeName = traineeUser?.UserName ?? $"Trainee {record.Enrollment.TraineeId}",
            CourseTitle = record.Enrollment.Session.Course.Title,
            TotalAmount = record.TotalAmount,
            PaidSoFar = paid,
            PaymentMethods = methods
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.Coordinator)]
    public async Task<IActionResult> RecordTransaction(RecordTransactionViewModel vm)
    {
        // Re-populate dropdown on invalid model
        if (!ModelState.IsValid)
        {
            vm.PaymentMethods = await _context.PaymentMethods
                .Select(m => new SelectListItem
                {
                    Value = m.PaymentMethodId.ToString(),
                    Text = m.PaymentMethod1
                }).ToListAsync();
            return View(vm);
        }

        var record = await _context.PaymentRecords
            .Include(p => p.Enrollment).ThenInclude(e => e.Trainee)
            .Include(p => p.Enrollment).ThenInclude(e => e.Session).ThenInclude(s => s.Course)
            .Include(p => p.PaymentTransactions)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.PaymentRecordId == vm.PaymentRecordId);

        if (record == null) return NotFound();

        var outstanding = record.TotalAmount - record.PaymentTransactions.Sum(t => t.Amount);
        if (vm.Amount > outstanding)
        {
            ModelState.AddModelError(nameof(vm.Amount),
                $"Amount cannot exceed the outstanding balance of {outstanding:C}.");
            vm.PaymentMethods = await _context.PaymentMethods
                .Select(m => new SelectListItem { Value = m.PaymentMethodId.ToString(), Text = m.PaymentMethod1 })
                .ToListAsync();
            vm.TotalAmount = record.TotalAmount;
            vm.PaidSoFar = record.PaymentTransactions.Sum(t => t.Amount);
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

        var now = DateTime.Now;
        _context.PaymentTransactions.Add(new PaymentTransaction
        {
            PaymentRecordId = vm.PaymentRecordId,
            CoordinatorId = coordinator.CoordinatorId,
            PaymentMethodId = vm.PaymentMethodId,
            PaymentMethod = method.PaymentMethod1,
            Amount = vm.Amount,
            PaymentDate = now,
            Notes = string.IsNullOrWhiteSpace(vm.Notes) ? null : vm.Notes.Trim(),
            CreatedAt = now
        });

        // Recalculate status
        var totalPaid = record.PaymentTransactions.Sum(t => t.Amount) + vm.Amount;
        string newStatusName;
        if (totalPaid >= record.TotalAmount)
            newStatusName = "Paid";
        else if (totalPaid > 0)
            newStatusName = "Partial";
        else
            newStatusName = "Unpaid";

        var newStatus = await _context.PaymentStatuses.FirstOrDefaultAsync(s => s.Status == newStatusName);
        if (newStatus != null)
        {
            record.StatusId = newStatus.StatusId;
            record.UpdatedAt = now;
        }

        await _context.SaveChangesAsync();

        // Notify the trainee
        var traineeUser = await _context.Users.FindAsync(record.Enrollment.Trainee.UserId);
        if (traineeUser != null)
        {
            var remaining = record.TotalAmount - totalPaid;
            var msg = totalPaid >= record.TotalAmount
                ? $"Your payment of {vm.Amount:C} for {record.Enrollment.Session.Course.Title} has been received. Your balance is fully settled."
                : $"Your payment of {vm.Amount:C} for {record.Enrollment.Session.Course.Title} has been received. Remaining balance: {remaining:C}.";

            await NotificationHelper.CreateAsync(_context, record.Enrollment.Trainee.UserId,
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

        var records = await _context.PaymentRecords
            .Include(p => p.Enrollment).ThenInclude(e => e.Session).ThenInclude(s => s.Course)
            .Include(p => p.PaymentTransactions)
            .Include(p => p.Status)
            .Where(p => p.Enrollment.TraineeId == trainee.TraineeId)
            .AsSplitQuery()
            .OrderByDescending(p => p.DueDate)
            .ToListAsync();

        var traineeUser = await _context.Users.FindAsync(userId);
        var name = traineeUser?.UserName ?? $"Trainee {trainee.TraineeId}";

        var rows = records.Select(p =>
        {
            var paid = p.PaymentTransactions.Sum(t => t.Amount);
            return new PaymentRecordRowViewModel
            {
                PaymentRecordId = p.PaymentRecordId,
                TraineeName = name,
                CourseTitle = p.Enrollment.Session.Course.Title,
                SessionDate = p.Enrollment.Session.SessionDate,
                TotalAmount = p.TotalAmount,
                PaidAmount = paid,
                Status = p.Status.Status,
                DueDate = p.DueDate,
                IsOverdue = p.Status.Status != "Paid" && p.DueDate.Date < DateTime.Today
            };
        }).ToList();

        return View(new MyPaymentsViewModel
        {
            Records = rows,
            TotalOwed = rows.Sum(r => r.TotalAmount),
            TotalPaid = rows.Sum(r => r.PaidAmount)
        });
    }
}
