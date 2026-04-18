using System;
using System.Collections.Generic;

namespace Web_API;

public partial class InstructorExpertise
{
    public int Id { get; set; }

    public int InstructorId { get; set; }

    public int SubjectAreaId { get; set; }

    public string ProficiencyLevel { get; set; } = null!;

    public virtual Instructor Instructor { get; set; } = null!;

    public virtual SubjectArea SubjectArea { get; set; } = null!;
}
