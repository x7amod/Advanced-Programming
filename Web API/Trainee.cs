using System;
using System.Collections.Generic;

namespace Web_API;

public partial class Trainee
{
    public int TraineeId { get; set; }

    public int UserId { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string Address { get; set; } = null!;

    public string EmergencyContact { get; set; } = null!;

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<TraineeCertification> TraineeCertifications { get; set; } = new List<TraineeCertification>();

    public virtual ICollection<TraineeCourseCompletion> TraineeCourseCompletions { get; set; } = new List<TraineeCourseCompletion>();

    public virtual AppUser User { get; set; } = null!;

    public virtual ICollection<Waitlist> Waitlists { get; set; } = new List<Waitlist>();
}
