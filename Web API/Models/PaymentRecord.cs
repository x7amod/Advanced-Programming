using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[Table("Payment_Record")]
public partial class PaymentRecord
{
    [Key]
    [Column("paymentRecordID")]
    public int PaymentRecordId { get; set; }

    [Column("enrollmentID")]
    public int EnrollmentId { get; set; }

    [Column("coordinatorID")]
    public int CoordinatorId { get; set; }

    [Column("statusID")]
    public int StatusId { get; set; }

    [Column("totalAmount", TypeName = "decimal(10, 2)")]
    public decimal TotalAmount { get; set; }

    [Column("dueDate", TypeName = "datetime")]
    public DateTime DueDate { get; set; }

    [Column("issuedDate", TypeName = "datetime")]
    public DateTime IssuedDate { get; set; }

    [Column("createdAt", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("updatedAt", TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey("CoordinatorId")]
    [InverseProperty("PaymentRecords")]
    public virtual Coordinator Coordinator { get; set; } = null!;

    [ForeignKey("EnrollmentId")]
    [InverseProperty("PaymentRecords")]
    public virtual Enrollment Enrollment { get; set; } = null!;

    [InverseProperty("PaymentRecord")]
    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();

    [ForeignKey("StatusId")]
    [InverseProperty("PaymentRecords")]
    public virtual PaymentStatus Status { get; set; } = null!;
}
