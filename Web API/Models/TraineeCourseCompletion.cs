using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[Table("Trainee_Course_Completion")]
public partial class TraineeCourseCompletion
{
    [Key]
    [Column("completionID")]
    public int CompletionId { get; set; }

    [Column("traineeID")]
    public int TraineeId { get; set; }

    [Column("courseID")]
    public int CourseId { get; set; }

    [Column("sessionID")]
    public int SessionId { get; set; }

    [Column("completionDate", TypeName = "datetime")]
    public DateTime CompletionDate { get; set; }

    [Column("result")]
    [StringLength(10)]
    public string Result { get; set; } = null!;

    [ForeignKey("CourseId")]
    [InverseProperty("TraineeCourseCompletions")]
    public virtual Course Course { get; set; } = null!;

    [ForeignKey("TraineeId")]
    [InverseProperty("TraineeCourseCompletions")]
    public virtual Trainee Trainee { get; set; } = null!;
}
