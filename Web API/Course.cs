using System;
using System.Collections.Generic;

namespace Web_API;

public partial class Course
{
    public int CourseId { get; set; }

    public int SubjectAreaId { get; set; }

    public int CategoryId { get; set; }

    public int? PrerequisiteCourseId { get; set; }

    public string CourseCode { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal DurationHours { get; set; }

    public int MaxCapacity { get; set; }

    public decimal EnrollmentFee { get; set; }

    public string? EquipmentRequirements { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<CertificationRequiredCourse> CertificationRequiredCourses { get; set; } = new List<CertificationRequiredCourse>();

    public virtual ICollection<CourseSession> CourseSessions { get; set; } = new List<CourseSession>();

    public virtual ICollection<Course> InversePrerequisiteCourse { get; set; } = new List<Course>();

    public virtual Course? PrerequisiteCourse { get; set; }

    public virtual SubjectArea SubjectArea { get; set; } = null!;

    public virtual ICollection<TraineeCourseCompletion> TraineeCourseCompletions { get; set; } = new List<TraineeCourseCompletion>();
}
