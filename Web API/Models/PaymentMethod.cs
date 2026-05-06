using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[Table("PaymentMethod")]
public partial class PaymentMethod
{
    [Key]
    [Column("paymentMethodID")]
    public int PaymentMethodId { get; set; }

    [Column("PaymentMethod")]
    [StringLength(15)]
    public string PaymentMethod1 { get; set; } = null!;

    [InverseProperty("PaymentMethodNavigation")]
    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}
