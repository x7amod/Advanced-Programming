using System;
using System.Collections.Generic;

namespace Web_API;

public partial class PaymentRecord
{
    public int PaymentRecordId { get; set; }

    public int EnrollmentId { get; set; }

    public int CoordinatorId { get; set; }

    public int StatusId { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime IssuedDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Coordinator Coordinator { get; set; } = null!;

    public virtual Enrollment Enrollment { get; set; } = null!;

    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();

    public virtual PaymentStatus Status { get; set; } = null!;
}
