using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC_Frontend.Models;

public class PaymentIndexViewModel
{
    public List<PaymentRecordRowViewModel> Records { get; set; } = new();
    public string? FilterStatus { get; set; }
    public List<SelectListItem> Statuses { get; set; } = new();
    public decimal TotalOutstanding { get; set; }
    public decimal TotalCollected { get; set; }
}

public class PaymentRecordRowViewModel
{
    public int PaymentRecordId { get; set; }
    public string TraineeName { get; set; } = "";
    public string CourseTitle { get; set; } = "";
    public DateTime SessionDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal OutstandingAmount => TotalAmount - PaidAmount;
    public string Status { get; set; } = "";
    public DateTime DueDate { get; set; }
    public bool IsOverdue { get; set; }
}

public class PaymentDetailsViewModel
{
    public int PaymentRecordId { get; set; }
    public string TraineeName { get; set; } = "";
    public string CourseTitle { get; set; } = "";
    public DateTime SessionDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal OutstandingAmount => TotalAmount - PaidAmount;
    public string Status { get; set; } = "";
    public DateTime DueDate { get; set; }
    public DateTime IssuedDate { get; set; }
    public bool IsOverdue { get; set; }
    public List<PaymentTransactionRowViewModel> Transactions { get; set; } = new();
}

public class PaymentTransactionRowViewModel
{
    public int TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "";
    public DateTime PaymentDate { get; set; }
    public string? Notes { get; set; }
}

public class CreatePaymentRecordViewModel
{
    public int EnrollmentId { get; set; }
    public string TraineeName { get; set; } = "";
    public string CourseTitle { get; set; } = "";
    public DateTime SessionDate { get; set; }

    [Required]
    [Range(0.01, 999999.99, ErrorMessage = "Amount must be greater than zero.")]
    [Display(Name = "Total Amount (BHD)")]
    public decimal TotalAmount { get; set; }

    [Required]
    [Display(Name = "Payment Due Date")]
    public DateTime DueDate { get; set; }
}

public class RecordTransactionViewModel
{
    public int PaymentRecordId { get; set; }
    public string TraineeName { get; set; } = "";
    public string CourseTitle { get; set; } = "";
    public decimal TotalAmount { get; set; }
    public decimal PaidSoFar { get; set; }
    public decimal Outstanding => TotalAmount - PaidSoFar;

    [Required]
    [Range(0.01, 999999.99, ErrorMessage = "Amount must be greater than zero.")]
    [Display(Name = "Amount Received (BHD)")]
    public decimal Amount { get; set; }

    [Required]
    [Display(Name = "Payment Method")]
    public int PaymentMethodId { get; set; }
    public List<SelectListItem> PaymentMethods { get; set; } = new();

    [StringLength(300)]
    public string? Notes { get; set; }
}

public class MyPaymentsViewModel
{
    public List<PaymentRecordRowViewModel> Records { get; set; } = new();
    public decimal TotalOwed { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Outstanding => TotalOwed - TotalPaid;
}
