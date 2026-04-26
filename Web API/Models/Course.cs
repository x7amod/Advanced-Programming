using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[Table("Course")]
public partial class Course
{
    [Key]
    [Column("courseID")]
    public int CourseId { get; set; }

    [Column("subjectAreaID")]
    public int SubjectAreaId { get; set; }

    [Column("categoryID")]
    public int CategoryId { get; set; }

    [Column("prerequisiteCourseID")]
    public int? PrerequisiteCourseId { get; set; }

    [Column("courseCode")]
    [StringLength(30)]
    public string CourseCode { get; set; } = null!;

    [Column("title")]
    [StringLength(150)]
    public string Title { get; set; } = null!;

    [Column("description")]
    [StringLength(1000)]
    public string? Description { get; set; }

    [Column("durationHours", TypeName = "decimal(5, 2)")]
    public decimal DurationHours { get; set; }

    [Column("maxCapacity")]
    public int MaxCapacity { get; set; }

    [Column("enrollmentFee", TypeName = "decimal(10, 2)")]
    public decimal EnrollmentFee { get; set; }

    [Column("equipmentRequirements")]
    [StringLength(500)]
    public string? EquipmentRequirements { get; set; }

    [Column("isActive")]
    public bool IsActive { get; set; }

    [Column("createdAt", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("updatedAt", TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Courses")]
    public virtual Category Category { get; set; } = null!;

    [InverseProperty("Course")]
    public virtual ICollection<CertificationRequiredCourse> CertificationRequiredCourses { get; set; } = new List<CertificationRequiredCourse>();

    [InverseProperty("Course")]
    public virtual ICollection<CourseSession> CourseSessions { get; set; } = new List<CourseSession>();

    [InverseProperty("PrerequisiteCourse")]
    public virtual ICollection<Course> InversePrerequisiteCourse { get; set; } = new List<Course>();

    [ForeignKey("PrerequisiteCourseId")]
    [InverseProperty("InversePrerequisiteCourse")]
    public virtual Course? PrerequisiteCourse { get; set; }

    [ForeignKey("SubjectAreaId")]
    [InverseProperty("Courses")]
    public virtual SubjectArea SubjectArea { get; set; } = null!;

    [InverseProperty("Course")]
    public virtual ICollection<TraineeCourseCompletion> TraineeCourseCompletions { get; set; } = new List<TraineeCourseCompletion>();
}
