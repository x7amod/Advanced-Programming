using System;
using System.Collections.Generic;

namespace Web_API;

public partial class Enrollment
{
    public int EnrollmentId { get; set; }

    public int SessionId { get; set; }

    public int TraineeId { get; set; }

    public int EnrollmentStatusId { get; set; }

    public DateTime EnrollmentDate { get; set; }

    public DateTime StatusChangedAt { get; set; }

    public string? DropReason { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();

    public virtual EnrollmentStatus EnrollmentStatus { get; set; } = null!;

    public virtual ICollection<PaymentRecord> PaymentRecords { get; set; } = new List<PaymentRecord>();

    public virtual CourseSession Session { get; set; } = null!;

    public virtual Trainee Trainee { get; set; } = null!;
}
