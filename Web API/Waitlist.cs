using System;
using System.Collections.Generic;

namespace Web_API;

public partial class Waitlist
{
    public int WaitlistId { get; set; }

    public int SessionId { get; set; }

    public int TraineeId { get; set; }

    public int Position { get; set; }

    public DateTime AddedAt { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? ExpiresAt { get; set; }

    public virtual CourseSession Session { get; set; } = null!;

    public virtual Trainee Trainee { get; set; } = null!;
}
