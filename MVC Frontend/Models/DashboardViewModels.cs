namespace MVC_Frontend.Models;

public class DashboardViewModel
{
    public string UserName { get; set; } = "";
    public string Role { get; set; } = "";
    public TraineeDashboardViewModel? Trainee { get; set; }
    public InstructorDashboardViewModel? Instructor { get; set; }
    public CoordinatorDashboardViewModel? Coordinator { get; set; }
}

// ─── Trainee ──────────────────────────────────────────────────────────────────

public class TraineeDashboardViewModel
{
    public int TotalEnrollments { get; set; }
    public int ActiveEnrollments { get; set; }
    public int CompletedEnrollments { get; set; }
    public int DroppedEnrollments { get; set; }
    public decimal TotalOwed { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Outstanding => TotalOwed - TotalPaid;
    public List<DashUpcomingSessionViewModel> UpcomingSessions { get; set; } = new();
    public List<DashCertProgressViewModel> CertProgress { get; set; } = new();
    public List<string> EnrollmentLabels { get; set; } = new();
    public List<int> EnrollmentCounts { get; set; } = new();
}

public class DashCertProgressViewModel
{
    public string TrackName { get; set; } = "";
    public int Completed { get; set; }
    public int Total { get; set; }
    public int Percentage => Total > 0 ? (int)Math.Round((double)Completed / Total * 100) : 0;
    public string Status { get; set; } = "";
}

// ─── Instructor ───────────────────────────────────────────────────────────────

public class InstructorDashboardViewModel
{
    public int UpcomingSessionCount { get; set; }
    public int CompletedSessionCount { get; set; }
    public int TotalTraineesAssessed { get; set; }
    public int PassCount { get; set; }
    public int FailCount { get; set; }
    public List<DashUpcomingSessionViewModel> UpcomingSessions { get; set; } = new();
    public List<string> MonthLabels { get; set; } = new();
    public List<int> MonthSessionCounts { get; set; } = new();
}

// ─── Coordinator ──────────────────────────────────────────────────────────────

public class CoordinatorDashboardViewModel
{
    public int TotalTrainees { get; set; }
    public int TotalInstructors { get; set; }
    public int TotalActiveCourses { get; set; }
    public int SessionsThisMonth { get; set; }
    public decimal TotalInvoiced { get; set; }
    public decimal TotalCollected { get; set; }
    public decimal TotalOutstanding => TotalInvoiced - TotalCollected;
    public List<string> EnrollmentLabels { get; set; } = new();
    public List<int> EnrollmentCounts { get; set; } = new();
    public List<string> MonthLabels { get; set; } = new();
    public List<int> MonthEnrollmentCounts { get; set; } = new();
    public List<DashTopCourseViewModel> TopCourses { get; set; } = new();
}

public class DashTopCourseViewModel
{
    public string CourseTitle { get; set; } = "";
    public int EnrollmentCount { get; set; }
}

// ─── Shared ───────────────────────────────────────────────────────────────────

public class DashUpcomingSessionViewModel
{
    public int SessionId { get; set; }
    public string CourseTitle { get; set; } = "";
    public DateTime SessionDate { get; set; }
    public DateTime StartTime { get; set; }
    public string? ClassroomName { get; set; }
}
