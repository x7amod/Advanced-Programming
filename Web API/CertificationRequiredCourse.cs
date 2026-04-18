using System;
using System.Collections.Generic;

namespace Web_API;

public partial class CertificationRequiredCourse
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public int CertificationId { get; set; }

    public int? SequenceOrder { get; set; }

    public bool IsMandatory { get; set; }

    public virtual CertificationTrack Certification { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;
}
