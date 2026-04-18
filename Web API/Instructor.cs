using System;
using System.Collections.Generic;

namespace Web_API;

public partial class Instructor
{
    public int InstructorId { get; set; }

    public int UserId { get; set; }

    public DateTime HireDate { get; set; }

    public string? Bio { get; set; }

    public virtual ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();

    public virtual ICollection<CourseSession> CourseSessions { get; set; } = new List<CourseSession>();

    public virtual ICollection<InstructorAvailability> InstructorAvailabilities { get; set; } = new List<InstructorAvailability>();

    public virtual ICollection<InstructorExpertise> InstructorExpertises { get; set; } = new List<InstructorExpertise>();

    public virtual AppUser User { get; set; } = null!;
}
