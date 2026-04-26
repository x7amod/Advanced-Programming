using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[Table("Coordinator")]
public partial class Coordinator
{
    [Key]
    [Column("coordinatorID")]
    public int CoordinatorId { get; set; }

    [Column("userID")]
    [StringLength(450)]
    public string UserId { get; set; } = null!;

    [Column("department")]
    [StringLength(100)]
    public string Department { get; set; } = null!;

    [InverseProperty("Coordinator")]
    public virtual ICollection<CourseSession> CourseSessions { get; set; } = new List<CourseSession>();

    [InverseProperty("Coordinator")]
    public virtual ICollection<PaymentRecord> PaymentRecords { get; set; } = new List<PaymentRecord>();

    [InverseProperty("Coordinator")]
    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}
