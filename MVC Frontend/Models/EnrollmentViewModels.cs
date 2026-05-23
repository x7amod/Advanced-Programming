using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC_Frontend.Models
{
    // Used in the trainee's "My Enrollments" list and the coordinator's "All Enrollments" table
    public class EnrollmentListItemViewModel
    {
        public int EnrollmentId { get; set; }
        public int TraineeId { get; set; }
        public string TraineeName { get; set; } = string.Empty;
        public int SessionId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public string? DropReason { get; set; }
    }

    // Full detail view for a single enrollment — used by both trainee and coordinator
    public class EnrollmentDetailsViewModel
    {
        public int EnrollmentId { get; set; }
        public int TraineeId { get; set; }
        public string TraineeName { get; set; } = string.Empty;
        public string TraineeEmail { get; set; } = string.Empty;
        public int SessionId { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public string ClassroomName { get; set; } = string.Empty;
        public int EnrollmentStatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public DateTime StatusChangedAt { get; set; }
        public string? DropReason { get; set; }

        // Assessments linked to this enrollment, shown in the details page
        public List<AssessmentSummaryViewModel> Assessments { get; set; } = new();
    }

    // Compact assessment summary embedded inside EnrollmentDetailsViewModel
    public class AssessmentSummaryViewModel
    {
        public int AssessmentId { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public DateTime AssessmentDate { get; set; }
    }

    // Shown to the trainee on the enrollment confirmation page before they submit
    public class EnrollSessionViewModel
    {
        public int SessionId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string? CourseDescription { get; set; }
        public decimal DurationHours { get; set; }
        public decimal EnrollmentFee { get; set; }
        public DateTime SessionDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public string ClassroomName { get; set; } = string.Empty;
        public string ClassroomLocation { get; set; } = string.Empty;
        public int CurrentEnrollment { get; set; }
        public int MaxCapacity { get; set; }

        // Computed remaining seats so the view doesn't need to do arithmetic
        public int SpotsLeft => MaxCapacity - CurrentEnrollment;
    }

    // Shown to the trainee on the cancellation confirmation page; includes optional drop reason
    public class CancelEnrollmentViewModel
    {
        public int EnrollmentId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public string StatusName { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Reason cannot exceed 255 characters.")]
        [Display(Name = "Reason for cancellation")]
        public string? DropReason { get; set; }
    }

    // Coordinator's "All Enrollments" page — combines the list data with filter inputs and dropdowns
    public class AllEnrollmentsViewModel
    {
        public List<EnrollmentListItemViewModel> Enrollments { get; set; } = new();

        // Current filter values (preserved across requests so the form stays filled)
        public int? FilterSessionId { get; set; }
        public int? FilterTraineeId { get; set; }
        public int? FilterStatusId { get; set; }
        public string? FilterDateFrom { get; set; }
        public string? FilterDateTo { get; set; }

        // Populated by the controller for the filter dropdowns
        public List<SelectListItem> Sessions { get; set; } = new();
        public List<SelectListItem> Trainees { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();
    }

    // Coordinator form for manually enrolling a trainee into a session
    public class ManualEnrollViewModel
    {
        [Required(ErrorMessage = "Please select a session.")]
        [Display(Name = "Session")]
        public int SessionId { get; set; }

        [Required(ErrorMessage = "Please select a trainee.")]
        [Display(Name = "Trainee")]
        public int TraineeId { get; set; }

        // Only scheduled sessions with available capacity are included
        public List<SelectListItem> Sessions { get; set; } = new();
        public List<SelectListItem> Trainees { get; set; } = new();
    }

    // POSTed when a coordinator changes an enrollment's status from the details page
    public class UpdateEnrollmentStatusViewModel
    {
        public int EnrollmentId { get; set; }

        [Required]
        public int NewStatusId { get; set; }

        // Required when moving to "Dropped"; optional otherwise
        [StringLength(255)]
        public string? DropReason { get; set; }
    }

    // Instructor view: one card per session they teach, with a roster of enrolled trainees
    public class InstructorSessionRosterViewModel
    {
        public int SessionId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public int CurrentEnrollment { get; set; }
        public int MaxCapacity { get; set; }
        public List<TraineeRosterItemViewModel> Trainees { get; set; } = new();
    }

    // One row in an instructor's session roster
    public class TraineeRosterItemViewModel
    {
        public int EnrollmentId { get; set; }
        public int TraineeId { get; set; }
        public string TraineeName { get; set; } = string.Empty;
        public string TraineeEmail { get; set; } = string.Empty;
        public string EnrollmentStatus { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public bool HasAssessment { get; set; }
    }
}
