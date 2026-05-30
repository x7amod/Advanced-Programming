namespace Reporting_App.Models
{
    public class LoginViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    // Overview
    public class OverviewViewModel
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

    //  Enrollment Stats 
    public class EnrollmentStatsViewModel
    {
        public EnrollmentSummary Summary { get; set; } = new();
        public List<EnrollmentByCourse> ByCourse { get; set; } = new();
    }

    public class EnrollmentSummary
    {
        public int TotalEnrollments { get; set; }
        public int TotalCompleted { get; set; }
        public int TotalDropped { get; set; }
        public int TotalActive { get; set; }
        public double OverallCompletionRate { get; set; }
    }

    public class EnrollmentByCourse
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

    //  Instructor Stats
    public class InstructorStatsViewModel
    {
        public InstructorSummary Summary { get; set; } = new();
        public List<InstructorData> ByInstructor { get; set; } = new();
    }

    public class InstructorSummary
    {
        public int TotalInstructors { get; set; }
        public double AvgSessionsPerInstructor { get; set; }
        public double AvgPassRate { get; set; }
    }

    public class InstructorData
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

    // Certification Stats 
    public class CertificationStatsViewModel
    {
        public CertificationSummary Summary { get; set; } = new();
        public List<CertificationByTrack> ByTrack { get; set; } = new();
    }

    public class CertificationSummary
    {
        public int TotalTracks { get; set; }
        public int TotalIssued { get; set; }
        public int TotalInProgress { get; set; }
        public int TotalExpired { get; set; }
    }

    public class CertificationByTrack
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

    //  Revenue Stats 
    public class RevenueStatsViewModel
    {
        public RevenueSummary Summary { get; set; } = new();
        public List<RevenueByCourse> ByCourse { get; set; } = new();
        public List<RevenueByMonth> ByMonth { get; set; } = new();
    }

    public class RevenueSummary
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalCollected { get; set; }
        public decimal TotalOutstanding { get; set; }
        public int OverdueCount { get; set; }
        public double CollectionRate { get; set; }
    }

    public class RevenueByCourse
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public decimal TotalInvoiced { get; set; }
        public decimal TotalCollected { get; set; }
        public decimal Outstanding { get; set; }
        public int OverdueCount { get; set; }
    }

    public class RevenueByMonth
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal TotalInvoiced { get; set; }
        public decimal TotalCollected { get; set; }
    }
}