using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[Table("Course_Session_Status")]
public partial class CourseSessionStatus
{
    [Key]
    [Column("StatusID")]
    public int StatusId { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = null!;

    [InverseProperty("Status")]
    public virtual ICollection<CourseSession> CourseSessions { get; set; } = new List<CourseSession>();
}
