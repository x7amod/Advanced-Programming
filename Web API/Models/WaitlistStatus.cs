using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[Table("Waitlist_Status")]
public partial class WaitlistStatus
{
    [Key]
    [Column("statusID")]
    public int StatusId { get; set; }

    [Column("status")]
    [StringLength(30)]
    public string Status { get; set; } = null!;

    [InverseProperty("StatusNavigation")]
    public virtual ICollection<Waitlist> Waitlists { get; set; } = new List<Waitlist>();
}
