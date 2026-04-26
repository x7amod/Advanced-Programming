using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[Table("Payment_Transaction")]
public partial class PaymentTransaction
{
    [Key]
    [Column("transactionID")]
    public int TransactionId { get; set; }

    [Column("paymentRecordID")]
    public int PaymentRecordId { get; set; }

    [Column("coordinatorID")]
    public int CoordinatorId { get; set; }

    [Column("amount", TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    [Column("paymentMethod")]
    [StringLength(20)]
    public string PaymentMethod { get; set; } = null!;

    [Column("paymentDate", TypeName = "datetime")]
    public DateTime PaymentDate { get; set; }

    [Column("notes")]
    [StringLength(300)]
    public string? Notes { get; set; }

    [Column("createdAt", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("CoordinatorId")]
    [InverseProperty("PaymentTransactions")]
    public virtual Coordinator Coordinator { get; set; } = null!;

    [ForeignKey("PaymentRecordId")]
    [InverseProperty("PaymentTransactions")]
    public virtual PaymentRecord PaymentRecord { get; set; } = null!;
}
