using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[Table("Instructor_Expertise")]
[Index("InstructorId", "SubjectAreaId", Name = "Instructor_Expertise_unique_comb", IsUnique = true)]
public partial class InstructorExpertise
{
    [Key]
    [Column("expertiseID")]
    public int ExpertiseId { get; set; }

    [Column("instructorID")]
    public int InstructorId { get; set; }

    [Column("subjectAreaID")]
    public int SubjectAreaId { get; set; }

    [Column("proficiencyLevel")]
    [StringLength(20)]
    public string ProficiencyLevel { get; set; } = null!;

    [ForeignKey("InstructorId")]
    [InverseProperty("InstructorExpertises")]
    public virtual Instructor Instructor { get; set; } = null!;

    [ForeignKey("SubjectAreaId")]
    [InverseProperty("InstructorExpertises")]
    public virtual SubjectArea SubjectArea { get; set; } = null!;
}
