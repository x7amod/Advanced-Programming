using System;
using System.Collections.Generic;

namespace Web_API;

public partial class Assessment
{
    public int AssessmentId { get; set; }

    public int EnrollmentId { get; set; }

    public int InstructorId { get; set; }

    public string Result { get; set; } = null!;

    public string? Remarks { get; set; }

    public DateTime AssessmentDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Enrollment Enrollment { get; set; } = null!;

    public virtual Instructor Instructor { get; set; } = null!;
}
