using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[Table("Enrollment_Status")]
public partial class EnrollmentStatus
{
    [Key]
    [Column("enrollmentStatusID")]
    public int EnrollmentStatusId { get; set; }

    [Column("status")]
    [StringLength(30)]
    public string Status { get; set; } = null!;

    [InverseProperty("EnrollmentStatus")]
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
