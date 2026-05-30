using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC_Frontend.Models;

// ── Instructor: Record Assessments ──────────────────────────────────────────

public class RecordAssessmentViewModel
{
    public int SessionId { get; set; }
    public string CourseTitle { get; set; } = "";
    public DateTime SessionDate { get; set; }
    public List<TraineeAssessmentRowViewModel> Trainees { get; set; } = new();
}

public class TraineeAssessmentRowViewModel
{
    public int EnrollmentId { get; set; }
    public int TraineeId { get; set; }
    public string TraineeName { get; set; } = "";
    public string? Result { get; set; }   // "Pass" | "Fail" — required, validated in controller
    public string? Remarks { get; set; }
}

// ── Instructor: My Submitted Assessments ────────────────────────────────────

public class MyAssessmentsViewModel
{
    public List<AssessmentItemViewModel> Assessments { get; set; } = new();
    public int? FilterSessionId { get; set; }
    public string? FilterResult { get; set; }
    public string? DateFrom { get; set; }
    public string? DateTo { get; set; }
    public List<SelectListItem> Sessions { get; set; } = new();
}

public class AssessmentItemViewModel
{
    public int AssessmentId { get; set; }
    public string TraineeName { get; set; } = "";
    public string InstructorName { get; set; } = "";
    public string CourseTitle { get; set; } = "";
    public DateTime SessionDate { get; set; }
    public int SessionId { get; set; }
    public string Result { get; set; } = "";
    public string? Remarks { get; set; }
    public DateTime AssessmentDate { get; set; }
}

// ── Coordinator: All Assessments ────────────────────────────────────────────

public class AllAssessmentsViewModel
{
    public List<AssessmentItemViewModel> Assessments { get; set; } = new();
    public int? FilterCourseId { get; set; }
    public int? FilterInstructorId { get; set; }
    public string? FilterResult { get; set; }
    public string? DateFrom { get; set; }
    public string? DateTo { get; set; }
    public List<SelectListItem> Courses { get; set; } = new();
    public List<SelectListItem> Instructors { get; set; } = new();
    public List<SessionPassRateViewModel> PassRates { get; set; } = new();
}

public class SessionPassRateViewModel
{
    public int SessionId { get; set; }
    public string CourseTitle { get; set; } = "";
    public DateTime SessionDate { get; set; }
    public int Passed { get; set; }
    public int Total { get; set; }
    public string Summary => $"{Passed} of {Total} passed";
}

// ── Trainee: My Results ──────────────────────────────────────────────────────

public class MyResultsViewModel
{
    public List<TraineeResultItemViewModel> Results { get; set; } = new();
}

public class TraineeResultItemViewModel
{
    public string CourseTitle { get; set; } = "";
    public DateTime SessionDate { get; set; }
    public string Result { get; set; } = "";
    public string? Remarks { get; set; }
    public DateTime CompletionDate { get; set; }
}
