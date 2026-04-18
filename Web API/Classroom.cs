using System;
using System.Collections.Generic;

namespace Web_API;

public partial class Classroom
{
    public int ClassroomId { get; set; }

    public string Name { get; set; } = null!;

    public string Location { get; set; } = null!;

    public string Building { get; set; } = null!;

    public string Floor { get; set; } = null!;

    public int Capacity { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<ClassroomEquipment> ClassroomEquipments { get; set; } = new List<ClassroomEquipment>();

    public virtual ICollection<CourseSession> CourseSessions { get; set; } = new List<CourseSession>();
}
