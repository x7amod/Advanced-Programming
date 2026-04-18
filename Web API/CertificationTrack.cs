using System;
using System.Collections.Generic;

namespace Web_API;

public partial class CertificationTrack
{
    public int CertificationId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? ValidityPeriod { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<CertificationRequiredCourse> CertificationRequiredCourses { get; set; } = new List<CertificationRequiredCourse>();

    public virtual ICollection<TraineeCertification> TraineeCertifications { get; set; } = new List<TraineeCertification>();
}
