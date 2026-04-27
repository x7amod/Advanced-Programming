using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[Table("Assessment")]
public partial class Assessment
{
    [Key]
    [Column("assessmentID")]
    public int AssessmentId { get; set; }

    [Column("enrollmentID")]
    public int EnrollmentId { get; set; }

    [Column("instructorID")]
    public int InstructorId { get; set; }

    [Column("result")]
    [StringLength(10)]
    public string Result { get; set; } = null!;

    [Column("remarks")]
    [StringLength(500)]
    public string? Remarks { get; set; }

    [Column("assessmentDate", TypeName = "datetime")]
    public DateTime AssessmentDate { get; set; }

    [Column("createdAt", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("EnrollmentId")]
    [InverseProperty("Assessments")]
    public virtual Enrollment Enrollment { get; set; } = null!;

    [ForeignKey("InstructorId")]
    [InverseProperty("Assessments")]
    public virtual Instructor Instructor { get; set; } = null!;
}
