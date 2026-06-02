namespace Web_API.Models
{
    public class OverviewReportDto
    {
        public int TotalEnrollments { get; set; }
        public int CompletedEnrollments { get; set; }
        public int TotalCertificationsIssued { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCollected { get; set; }
        public decimal TotalOutstanding { get; set; }
        public double OverallPassRate { get; set; }
        public double CompletionRate { get; set; }
        public int TotalTrainees { get; set; }
        public int TotalInstructors { get; set; }
        public int TotalCourses { get; set; }
        public int TotalSessions { get; set; }
    }

    public class EnrollmentStatsReportDto
    {
        public EnrollmentSummaryDto Summary { get; set; } = new();
        public List<EnrollmentByCourseDto> ByCourse { get; set; } = new();
    }

    public class EnrollmentSummaryDto
    {
        public int TotalEnrollments { get; set; }
        public int TotalCompleted { get; set; }
        public int TotalDropped { get; set; }
        public int TotalActive { get; set; }
        public double OverallCompletionRate { get; set; }
    }

    public class EnrollmentByCourseDto
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int TotalEnrollments { get; set; }
        public int Completed { get; set; }
        public int Dropped { get; set; }
        public int Active { get; set; }
        public double CompletionRate { get; set; }
    }

    public class InstructorStatsReportDto
    {
        public InstructorSummaryDto Summary { get; set; } = new();
        public List<InstructorDataDto> ByInstructor { get; set; } = new();
    }

    public class InstructorSummaryDto
    {
        public int TotalInstructors { get; set; }
        public double AvgSessionsPerInstructor { get; set; }
        public double AvgPassRate { get; set; }
    }

    public class InstructorDataDto
    {
        public int InstructorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TotalSessions { get; set; }
        public int CompletedSessions { get; set; }
        public int TotalTrainees { get; set; }
        public int TotalAssessments { get; set; }
        public int PassedAssessments { get; set; }
        public double AvgPassRate { get; set; }
    }

    public class CertificationStatsReportDto
    {
        public CertificationSummaryDto Summary { get; set; } = new();
        public List<CertificationByTrackDto> ByTrack { get; set; } = new();
    }

    public class CertificationSummaryDto
    {
        public int TotalTracks { get; set; }
        public int TotalIssued { get; set; }
        public int TotalInProgress { get; set; }
        public int TotalExpired { get; set; }
    }

    public class CertificationByTrackDto
    {
        public int TrackId { get; set; }
        public string TrackName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int RequiredCoursesCount { get; set; }
        public int TotalEnrolled { get; set; }
        public int Issued { get; set; }
        public int Eligible { get; set; }
        public int InProgress { get; set; }
        public int Expired { get; set; }
        public double CompletionRate { get; set; }
    }

    public class RevenueStatsReportDto
    {
        public RevenueSummaryDto Summary { get; set; } = new();
        public List<RevenueByCourseDto> ByCourse { get; set; } = new();
        public List<RevenueByMonthDto> ByMonth { get; set; } = new();
    }

    public class RevenueSummaryDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalCollected { get; set; }
        public decimal TotalOutstanding { get; set; }
        public int OverdueCount { get; set; }
        public double CollectionRate { get; set; }
    }

    public class RevenueByCourseDto
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public decimal TotalInvoiced { get; set; }
        public decimal TotalCollected { get; set; }
        public decimal Outstanding { get; set; }
        public int OverdueCount { get; set; }
    }

    public class RevenueByMonthDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal TotalInvoiced { get; set; }
        public decimal TotalCollected { get; set; }
    }
}
