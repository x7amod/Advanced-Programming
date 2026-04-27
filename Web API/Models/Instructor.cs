using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[Table("Instructor")]
public partial class Instructor
{
    [Key]
    [Column("instructorID")]
    public int InstructorId { get; set; }

    [Column("userID")]
    [StringLength(450)]
    public string UserId { get; set; } = null!;

    [Column("hireDate", TypeName = "datetime")]
    public DateTime HireDate { get; set; }

    [Column("bio")]
    [StringLength(500)]
    public string? Bio { get; set; }

    [InverseProperty("Instructor")]
    public virtual ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();

    [InverseProperty("Instructor")]
    public virtual ICollection<CourseSession> CourseSessions { get; set; } = new List<CourseSession>();

    [InverseProperty("Instructor")]
    public virtual ICollection<InstructorAvailability> InstructorAvailabilities { get; set; } = new List<InstructorAvailability>();

    [InverseProperty("Instructor")]
    public virtual ICollection<InstructorExpertise> InstructorExpertises { get; set; } = new List<InstructorExpertise>();
}
