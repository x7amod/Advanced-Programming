using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[PrimaryKey("AvailabilityId", "InstructorId")]
[Table("Instructor_Availability")]
public partial class InstructorAvailability
{
    [Key]
    [Column("availabilityID")]
    public int AvailabilityId { get; set; }

    [Key]
    [Column("instructorID")]
    public int InstructorId { get; set; }

    [Column("dayOfWeek")]
    public int DayOfWeek { get; set; }

    [Column("startTime", TypeName = "datetime")]
    public DateTime StartTime { get; set; }

    [Column("endTime", TypeName = "datetime")]
    public DateTime EndTime { get; set; }

    [Column("effectiveFrom", TypeName = "datetime")]
    public DateTime EffectiveFrom { get; set; }

    [Column("effectiveTo", TypeName = "datetime")]
    public DateTime? EffectiveTo { get; set; }

    [Column("isRecurring")]
    public bool IsRecurring { get; set; }

    [ForeignKey("InstructorId")]
    [InverseProperty("InstructorAvailabilities")]
    public virtual Instructor Instructor { get; set; } = null!;
}
