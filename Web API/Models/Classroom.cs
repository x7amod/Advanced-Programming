using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[Table("Classroom")]
public partial class Classroom
{
    [Key]
    [Column("classroomID")]
    public int ClassroomId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("location")]
    [StringLength(200)]
    public string Location { get; set; } = null!;

    [Column("building")]
    [StringLength(100)]
    public string Building { get; set; } = null!;

    [Column("floor")]
    [StringLength(20)]
    public string Floor { get; set; } = null!;

    [Column("capacity")]
    public int Capacity { get; set; }

    [Column("isActive")]
    public bool IsActive { get; set; }

    [InverseProperty("Classroom")]
    public virtual ICollection<ClassroomEquipment> ClassroomEquipments { get; set; } = new List<ClassroomEquipment>();

    [InverseProperty("Classroom")]
    public virtual ICollection<CourseSession> CourseSessions { get; set; } = new List<CourseSession>();
}
