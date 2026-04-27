using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[Table("Enrollment")]
public partial class Enrollment
{
    [Key]
    [Column("enrollmentID")]
    public int EnrollmentId { get; set; }

    [Column("sessionID")]
    public int SessionId { get; set; }

    [Column("traineeID")]
    public int TraineeId { get; set; }

    [Column("enrollmentStatusID")]
    public int EnrollmentStatusId { get; set; }

    [Column("enrollmentDate", TypeName = "datetime")]
    public DateTime EnrollmentDate { get; set; }

    [Column("statusChangedAt", TypeName = "datetime")]
    public DateTime StatusChangedAt { get; set; }

    [Column("dropReason")]
    [StringLength(255)]
    public string? DropReason { get; set; }

    [Column("createdAt", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("updatedAt", TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; }

    [InverseProperty("Enrollment")]
    public virtual ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();

    [ForeignKey("EnrollmentStatusId")]
    [InverseProperty("Enrollments")]
    public virtual EnrollmentStatus EnrollmentStatus { get; set; } = null!;

    [InverseProperty("Enrollment")]
    public virtual ICollection<PaymentRecord> PaymentRecords { get; set; } = new List<PaymentRecord>();

    [ForeignKey("SessionId")]
    [InverseProperty("Enrollments")]
    public virtual CourseSession Session { get; set; } = null!;

    [ForeignKey("TraineeId")]
    [InverseProperty("Enrollments")]
    public virtual Trainee Trainee { get; set; } = null!;
}
