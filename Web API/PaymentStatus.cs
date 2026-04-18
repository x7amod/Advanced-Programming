using System;
using System.Collections.Generic;

namespace Web_API;

public partial class PaymentStatus
{
    public int StatusId { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<PaymentRecord> PaymentRecords { get; set; } = new List<PaymentRecord>();
}
