using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[Table("Trainee")]
public partial class Trainee
{
    [Key]
    [Column("traineeID")]
    public int TraineeId { get; set; }

    [Column("userID")]
    [StringLength(450)]
    public string UserId { get; set; } = null!;

    [Column("dateOfBirth", TypeName = "datetime")]
    public DateTime DateOfBirth { get; set; }

    [Column("address")]
    [StringLength(50)]
    public string Address { get; set; } = null!;

    [Column("emergencyContact")]
    [StringLength(50)]
    public string EmergencyContact { get; set; } = null!;

    [InverseProperty("Trainee")]
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    [InverseProperty("Trainee")]
    public virtual ICollection<TraineeCertification> TraineeCertifications { get; set; } = new List<TraineeCertification>();

    [InverseProperty("Trainee")]
    public virtual ICollection<TraineeCourseCompletion> TraineeCourseCompletions { get; set; } = new List<TraineeCourseCompletion>();

    [InverseProperty("Trainee")]
    public virtual ICollection<Waitlist> Waitlists { get; set; } = new List<Waitlist>();
}
