using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[PrimaryKey("EquipmentId", "ClassroomId")]
[Table("Classroom_Equipment")]
public partial class ClassroomEquipment
{
    [Key]
    [Column("equipmentID")]
    public int EquipmentId { get; set; }

    [Key]
    [Column("classroomID")]
    public int ClassroomId { get; set; }

    [Column("equipmentType")]
    [StringLength(50)]
    public string EquipmentType { get; set; } = null!;

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    [ForeignKey("ClassroomId")]
    [InverseProperty("ClassroomEquipments")]
    public virtual Classroom Classroom { get; set; } = null!;
}
