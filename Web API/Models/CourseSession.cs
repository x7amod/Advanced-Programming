using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

[Table("Course_Session")]
public partial class CourseSession
{
    [Key]
    [Column("sessionID")]
    public int SessionId { get; set; }

    [Column("coordinatorID")]
    public int CoordinatorId { get; set; }

    [Column("classroomID")]
    public int ClassroomId { get; set; }

    [Column("courseID")]
    public int CourseId { get; set; }

    [Column("instructorID")]
    public int InstructorId { get; set; }

    [Column("StatusID")]
    public int StatusId { get; set; }

    [Column("sessionDate", TypeName = "datetime")]
    public DateTime SessionDate { get; set; }

    [Column("startTime", TypeName = "datetime")]
    public DateTime StartTime { get; set; }

    [Column("endTime", TypeName = "datetime")]
    public DateTime EndTime { get; set; }

    [Column("currentEnrollment")]
    public int CurrentEnrollment { get; set; }

    [Column("maxCapacity")]
    public int MaxCapacity { get; set; }

    [Column("createdAt", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("updatedAt", TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey("ClassroomId")]
    [InverseProperty("CourseSessions")]
    public virtual Classroom Classroom { get; set; } = null!;

    [ForeignKey("CoordinatorId")]
    [InverseProperty("CourseSessions")]
    public virtual Coordinator Coordinator { get; set; } = null!;

    [ForeignKey("CourseId")]
    [InverseProperty("CourseSessions")]
    public virtual Course Course { get; set; } = null!;

    [InverseProperty("Session")]
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    [ForeignKey("InstructorId")]
    [InverseProperty("CourseSessions")]
    public virtual Instructor Instructor { get; set; } = null!;

    [ForeignKey("StatusId")]
    [InverseProperty("CourseSessions")]
    public virtual CourseSessionStatus Status { get; set; } = null!;

    [InverseProperty("Session")]
    public virtual ICollection<Waitlist> Waitlists { get; set; } = new List<Waitlist>();
}
