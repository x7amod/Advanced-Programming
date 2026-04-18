using System;
using System.Collections.Generic;

namespace Web_API;

public partial class SubjectArea
{
    public int SubjectAreaId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<InstructorExpertise> InstructorExpertises { get; set; } = new List<InstructorExpertise>();
}
