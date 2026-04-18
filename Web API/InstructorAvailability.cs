using System;
using System.Collections.Generic;

namespace Web_API;

public partial class InstructorAvailability
{
    public int AvailabilityId { get; set; }

    public int InstructorId { get; set; }

    public int DayOfWeek { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsRecurring { get; set; }

    public virtual Instructor Instructor { get; set; } = null!;
}
