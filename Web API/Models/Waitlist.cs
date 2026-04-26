using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[Table("Waitlist")]
[Index("SessionId", "TraineeId", Name = "Waitlist_unique_comb", IsUnique = true)]
public partial class Waitlist
{
    [Key]
    [Column("waitlistID")]
    public int WaitlistId { get; set; }

    [Column("sessionID")]
    public int SessionId { get; set; }

    [Column("traineeID")]
    public int TraineeId { get; set; }

    [Column("statusID")]
    public int StatusId { get; set; }

    [Column("position")]
    public int Position { get; set; }

    [Column("addedAt", TypeName = "datetime")]
    public DateTime AddedAt { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = null!;

    [Column("expiresAt", TypeName = "datetime")]
    public DateTime? ExpiresAt { get; set; }

    [ForeignKey("SessionId")]
    [InverseProperty("Waitlists")]
    public virtual CourseSession Session { get; set; } = null!;

    [ForeignKey("StatusId")]
    [InverseProperty("Waitlists")]
    public virtual WaitlistStatus StatusNavigation { get; set; } = null!;

    [ForeignKey("TraineeId")]
    [InverseProperty("Waitlists")]
    public virtual Trainee Trainee { get; set; } = null!;
}
