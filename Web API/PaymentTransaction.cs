using System;
using System.Collections.Generic;

namespace Web_API;

public partial class PaymentTransaction
{
    public int TransactionId { get; set; }

    public int PaymentRecordId { get; set; }

    public int CoordinatorId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public DateTime PaymentDate { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Coordinator Coordinator { get; set; } = null!;

    public virtual PaymentRecord PaymentRecord { get; set; } = null!;
}
