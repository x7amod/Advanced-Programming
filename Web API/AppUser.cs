using System;
using System.Collections.Generic;

namespace Web_API;

public partial class AppUser
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Password { get; set; }

    public string Phone { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Coordinator> Coordinators { get; set; } = new List<Coordinator>();

    public virtual ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual UserRole Role { get; set; } = null!;

    public virtual ICollection<Trainee> Trainees { get; set; } = new List<Trainee>();
}
