using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC_Frontend.Models
{
    // One row in the "All Assessments" table (coordinator) or "My Assessments" list (trainee)
    public class AssessmentListItemViewModel
    {
        public int AssessmentId { get; set; }
        public int EnrollmentId { get; set; }
        public int TraineeId { get; set; }
        public string TraineeName { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public string InstructorName { get; set; } = string.Empty;

        // Result stores "Pass", "Fail", or "Pending" (pending = not yet graded)
        public string Result { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public DateTime AssessmentDate { get; set; }
    }

    // Full detail view for a single assessment — used by all roles
    public class AssessmentDetailsViewModel
    {
        public int AssessmentId { get; set; }
        public int EnrollmentId { get; set; }
        public int TraineeId { get; set; }
        public string TraineeName { get; set; } = string.Empty;
        public string TraineeEmail { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public DateTime AssessmentDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string EnrollmentStatus { get; set; } = string.Empty;
    }

    // Coordinator form for creating or editing an assessment
    public class AssessmentFormViewModel
    {
        // 0 means "create new"; non-zero means "edit existing"
        public int AssessmentId { get; set; }

        [Required(ErrorMessage = "Enrollment is required.")]
        [Display(Name = "Enrollment")]
        public int EnrollmentId { get; set; }

        [Required(ErrorMessage = "Please assign an instructor.")]
        [Display(Name = "Instructor")]
        public int InstructorId { get; set; }

        // Valid values: "Pending", "Pass", "Fail" (max 10 chars to match DB column)
        [Required(ErrorMessage = "Please select a result.")]
        [StringLength(10)]
        [Display(Name = "Result")]
        public string Result { get; set; } = "Pending";

        [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters.")]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        [Required(ErrorMessage = "Assessment date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Assessment Date")]
        public DateTime AssessmentDate { get; set; } = DateTime.Today;

        // Read-only display fields — shown on the form so the coordinator knows the context
        public string TraineeName { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }

        // Populated by the controller for the instructor dropdown
        public List<SelectListItem> Instructors { get; set; } = new();

        // Populated when creating from the AllEnrollments page (allows picking which enrollment)
        public List<SelectListItem> Enrollments { get; set; } = new();
    }

    // Coordinator's "All Assessments" page — list + filter state
    public class AllAssessmentsViewModel
    {
        public List<AssessmentListItemViewModel> Assessments { get; set; } = new();

        // Current filter values preserved so the filter bar stays populated
        public int? FilterSessionId { get; set; }
        public int? FilterInstructorId { get; set; }
        public string? FilterResult { get; set; }

        // Populated by the controller for filter dropdowns
        public List<SelectListItem> Sessions { get; set; } = new();
        public List<SelectListItem> Instructors { get; set; } = new();
    }

    // Instructor's "Session Results" page — shows all trainee assessments for one session
    public class SessionResultsViewModel
    {
        public int SessionId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public string SessionStatus { get; set; } = string.Empty;
        public List<TraineeResultItemViewModel> Results { get; set; } = new();

        // Computed summary counts used in the summary banner
        public int TotalEnrolled { get; set; }
        public int PassCount { get; set; }
        public int FailCount { get; set; }
        public int PendingCount { get; set; }
    }

    // One row in a session's results table (instructor view)
    public class TraineeResultItemViewModel
    {
        public int EnrollmentId { get; set; }
        public int TraineeId { get; set; }
        public string TraineeName { get; set; } = string.Empty;
        public string EnrollmentStatus { get; set; } = string.Empty;

        // Null means no assessment record exists yet
        public int? AssessmentId { get; set; }
        public string Result { get; set; } = "Pending";
        public string? Remarks { get; set; }
        public DateTime? AssessmentDate { get; set; }
    }
}
