using System;
using System.Collections.Generic;

namespace Web_API;

public partial class CourseSession
{
    public int SessionId { get; set; }

    public int CoordinatorId { get; set; }

    public int ClassroomId { get; set; }

    public int CourseId { get; set; }

    public int InstructorId { get; set; }

    public DateTime SessionDate { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int CurrentEnrollment { get; set; }

    public int MaxCapacity { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Classroom Classroom { get; set; } = null!;

    public virtual Coordinator Coordinator { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual Instructor Instructor { get; set; } = null!;

    public virtual ICollection<Waitlist> Waitlists { get; set; } = new List<Waitlist>();
}
