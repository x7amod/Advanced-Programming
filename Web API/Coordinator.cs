using System;
using System.Collections.Generic;

namespace Web_API;

public partial class Coordinator
{
    public int CoordinatorId { get; set; }

    public int UserId { get; set; }

    public string Department { get; set; } = null!;

    public virtual ICollection<CourseSession> CourseSessions { get; set; } = new List<CourseSession>();

    public virtual ICollection<PaymentRecord> PaymentRecords { get; set; } = new List<PaymentRecord>();

    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();

    public virtual AppUser User { get; set; } = null!;
}
