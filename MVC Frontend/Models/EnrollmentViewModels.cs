using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC_Frontend.Models;

// ── Browse page ──────────────────────────────────────────────────────────────

public class BrowseSessionsViewModel
{
    public List<SessionCardViewModel> Sessions { get; set; } = new();
    public List<SelectListItem> SubjectAreas { get; set; } = new();
    public List<SelectListItem> Categories { get; set; } = new();
    public int? SubjectAreaId { get; set; }
    public int? CategoryId { get; set; }
    public string? DateFrom { get; set; }
    public string? DateTo { get; set; }
}

public class SessionCardViewModel
{
    public int SessionId { get; set; }
    public string CourseTitle { get; set; } = "";
    public string InstructorName { get; set; } = "";
    public string ClassroomName { get; set; } = "";
    public DateTime SessionDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int RemainingSpots { get; set; }
    public int MaxCapacity { get; set; }
    public decimal EnrollmentFee { get; set; }
    public bool IsFull { get; set; }
    public bool IsAlreadyEnrolled { get; set; }
    public bool IsWaitlisted { get; set; }
}

// ── My Enrollments page ──────────────────────────────────────────────────────

public class MyEnrollmentsViewModel
{
    public List<EnrollmentItemViewModel> UpcomingEnrollments { get; set; } = new();
    public List<EnrollmentItemViewModel> PastEnrollments { get; set; } = new();
    public List<EnrollmentItemViewModel> DroppedEnrollments { get; set; } = new();
    public List<WaitlistItemViewModel> WaitlistEntries { get; set; } = new();
}

public class EnrollmentItemViewModel
{
    public int EnrollmentId { get; set; }
    public int SessionId { get; set; }
    public string CourseTitle { get; set; } = "";
    public string InstructorName { get; set; } = "";
    public DateTime SessionDate { get; set; }
    public DateTime StartTime { get; set; }
    public string Status { get; set; } = "";
    public string? AssessmentResult { get; set; }
    public bool CanDrop { get; set; }
}

public class WaitlistItemViewModel
{
    public int WaitlistId { get; set; }
    public int SessionId { get; set; }
    public string CourseTitle { get; set; } = "";
    public DateTime SessionDate { get; set; }
    public int Position { get; set; }
    public string Status { get; set; } = "";
}

// ── Drop confirmation ────────────────────────────────────────────────────────

public class DropEnrollmentViewModel
{
    public int EnrollmentId { get; set; }
    public string CourseTitle { get; set; } = "";
    public DateTime SessionDate { get; set; }

    [Required(ErrorMessage = "Please provide a reason for dropping.")]
    [StringLength(255, ErrorMessage = "Reason cannot exceed 255 characters.")]
    [Display(Name = "Drop Reason")]
    public string DropReason { get; set; } = "";
}

// ── Coordinator: Manage all enrollments ──────────────────────────────────────

public class ManageEnrollmentsViewModel
{
    public List<ManageEnrollmentItemViewModel> Enrollments { get; set; } = new();
    public int? FilterSessionId { get; set; }
    public int? FilterStatusId { get; set; }
    public string? FilterTraineeName { get; set; }
    public string? DateFrom { get; set; }
    public string? DateTo { get; set; }
    public List<SelectListItem> Sessions { get; set; } = new();
    public List<SelectListItem> Statuses { get; set; } = new();
    public List<SessionCapacitySummaryViewModel> SessionSummaries { get; set; } = new();
}

public class ManageEnrollmentItemViewModel
{
    public int EnrollmentId { get; set; }
    public string TraineeName { get; set; } = "";
    public string CourseTitle { get; set; } = "";
    public DateTime SessionDate { get; set; }
    public int SessionId { get; set; }
    public string Status { get; set; } = "";
    public DateTime EnrollmentDate { get; set; }
    public bool CanConfirm { get; set; }
    public bool HasPaymentRecord { get; set; }
}

public class SessionCapacitySummaryViewModel
{
    public int SessionId { get; set; }
    public string CourseTitle { get; set; } = "";
    public DateTime SessionDate { get; set; }
    public int CurrentEnrollment { get; set; }
    public int MaxCapacity { get; set; }
}

// ── Instructor: My Sessions list ─────────────────────────────────────────────

public class InstructorSessionsViewModel
{
    public List<InstructorSessionItemViewModel> Sessions { get; set; } = new();
}

public class InstructorSessionItemViewModel
{
    public int SessionId { get; set; }
    public string CourseTitle { get; set; } = "";
    public DateTime SessionDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string SessionStatus { get; set; } = "";
    public int EnrollmentCount { get; set; }
}

// ── Instructor: Session Roster ───────────────────────────────────────────────

public class SessionRosterViewModel
{
    public int SessionId { get; set; }
    public string CourseTitle { get; set; } = "";
    public DateTime SessionDate { get; set; }
    public string SessionStatus { get; set; } = "";
    public List<RosterTraineeViewModel> Trainees { get; set; } = new();
}

public class RosterTraineeViewModel
{
    public int EnrollmentId { get; set; }
    public string TraineeName { get; set; } = "";
    public string EnrollmentStatus { get; set; } = "";
}
