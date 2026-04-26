using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[Table("Trainee_Certification")]
[Index("TraineeId", "CertificationTrackId", Name = "Trainee_Certification_unique_comb", IsUnique = true)]
public partial class TraineeCertification
{
    [Key]
    [Column("traineeCertID")]
    public int TraineeCertId { get; set; }

    [Column("traineeID")]
    public int TraineeId { get; set; }

    [Column("certificationTrackID")]
    public int CertificationTrackId { get; set; }

    [Column("statusID")]
    public int StatusId { get; set; }

    [Column("eligibleDate", TypeName = "datetime")]
    public DateTime? EligibleDate { get; set; }

    [Column("certificateIssuedDate", TypeName = "datetime")]
    public DateTime? CertificateIssuedDate { get; set; }

    [Column("certificateNumber")]
    [StringLength(50)]
    public string? CertificateNumber { get; set; }

    [Column("expiryDate", TypeName = "datetime")]
    public DateTime? ExpiryDate { get; set; }

    [ForeignKey("CertificationTrackId")]
    [InverseProperty("TraineeCertifications")]
    public virtual CertificationTrack CertificationTrack { get; set; } = null!;

    [ForeignKey("StatusId")]
    [InverseProperty("TraineeCertifications")]
    public virtual CertificationStatus Status { get; set; } = null!;

    [ForeignKey("TraineeId")]
    [InverseProperty("TraineeCertifications")]
    public virtual Trainee Trainee { get; set; } = null!;
}
