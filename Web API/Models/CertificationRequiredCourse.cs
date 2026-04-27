using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[Table("Certification_Required_Course")]
[Index("CourseId", "CertificationTrackId", Name = "Certification_Required_Course_unique_comb", IsUnique = true)]
public partial class CertificationRequiredCourse
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("courseID")]
    public int CourseId { get; set; }

    [Column("certificationTrackID")]
    public int CertificationTrackId { get; set; }

    [Column("sequenceOrder")]
    public int? SequenceOrder { get; set; }

    [Column("isMandatory")]
    public bool IsMandatory { get; set; }

    [ForeignKey("CertificationTrackId")]
    [InverseProperty("CertificationRequiredCourses")]
    public virtual CertificationTrack CertificationTrack { get; set; } = null!;

    [ForeignKey("CourseId")]
    [InverseProperty("CertificationRequiredCourses")]
    public virtual Course Course { get; set; } = null!;
}
