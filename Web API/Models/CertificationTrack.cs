using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[Table("Certification_Track")]
public partial class CertificationTrack
{
    [Key]
    [Column("certificationTrackID")]
    public int CertificationTrackId { get; set; }

    [Column("name")]
    [StringLength(150)]
    public string Name { get; set; } = null!;

    [Column("description")]
    [StringLength(1000)]
    public string? Description { get; set; }

    [Column("validityPeriod")]
    public int? ValidityPeriod { get; set; }

    [Column("isActive")]
    public bool IsActive { get; set; }

    [Column("createdAt", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("updatedAt", TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; }

    [InverseProperty("CertificationTrack")]
    public virtual ICollection<CertificationRequiredCourse> CertificationRequiredCourses { get; set; } = new List<CertificationRequiredCourse>();

    [InverseProperty("CertificationTrack")]
    public virtual ICollection<TraineeCertification> TraineeCertifications { get; set; } = new List<TraineeCertification>();
}
