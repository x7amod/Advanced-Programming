using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[Table("Notification")]
public partial class Notification
{
    [Key]
    [Column("notificationID")]
    public int NotificationId { get; set; }

    [Column("userID")]
    [StringLength(450)]
    public string UserId { get; set; } = null!;

    [Column("title")]
    [StringLength(150)]
    public string Title { get; set; } = null!;

    [Column("message")]
    [StringLength(1000)]
    public string Message { get; set; } = null!;

    [Column("type")]
    [StringLength(50)]
    public string Type { get; set; } = null!;

    [Column("relatedEntityType")]
    [StringLength(50)]
    public string? RelatedEntityType { get; set; }

    [Column("isRead")]
    public bool IsRead { get; set; }

    [Column("createdAt", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("readAt", TypeName = "datetime")]
    public DateTime? ReadAt { get; set; }
}
