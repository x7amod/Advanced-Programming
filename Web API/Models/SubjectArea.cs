using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[Table("Subject_Area")]
public partial class SubjectArea
{
    [Key]
    [Column("subjectAreaID")]
    public int SubjectAreaId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }

    [InverseProperty("SubjectArea")]
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    [InverseProperty("SubjectArea")]
    public virtual ICollection<InstructorExpertise> InstructorExpertises { get; set; } = new List<InstructorExpertise>();
}
