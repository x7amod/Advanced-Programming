using System;
using System.Collections.Generic;

namespace Web_API;

public partial class EnrollmentStatus
{
    public int EnrollmentStatusId { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
