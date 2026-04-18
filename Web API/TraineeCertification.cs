using System;
using System.Collections.Generic;

namespace Web_API;

public partial class TraineeCertification
{
    public int TraineeCertId { get; set; }

    public int TraineeId { get; set; }

    public int CertificationId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? EligibleDate { get; set; }

    public DateTime? CertificateIssuedDate { get; set; }

    public string? CertificateNumber { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public virtual CertificationTrack Certification { get; set; } = null!;

    public virtual Trainee Trainee { get; set; } = null!;
}
