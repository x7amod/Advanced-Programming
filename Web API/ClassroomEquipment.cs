using System;
using System.Collections.Generic;

namespace Web_API;

public partial class ClassroomEquipment
{
    public int EquipmentId { get; set; }

    public int ClassroomId { get; set; }

    public string EquipmentType { get; set; } = null!;

    public int Quantity { get; set; }

    public string? Description { get; set; }

    public virtual Classroom Classroom { get; set; } = null!;
}
