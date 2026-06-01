using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_API.Models;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize(Roles = AppRoles.Coordinator)]
    public class ReportsController : ControllerBase
    {
        private readonly TrainingInstituteDBContext _context;

        public ReportsController(TrainingInstituteDBContext context)
        {
            _context = context;
        }

        // GET /api/reports/enrollments
        [HttpGet("enrollments")]
        [ProducesResponseType(typeof(EnrollmentStatsReportDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<EnrollmentStatsReportDto>> GetEnrollmentStats()
        {
            var byCourse = await _context.Enrollments
                .AsNoTracking()
                .GroupBy(e => new
                {
                    CourseId = e.Session.CourseId,
                    CourseTitle = e.Session.Course.Title,
                    CourseCode = e.Session.Course.CourseCode,
                    CategoryName = e.Session.Course.Category.Name
                })
                .Select(g => new
                {
                    g.Key.CourseId,
                    g.Key.CourseTitle,
                    g.Key.CourseCode,
                    g.Key.CategoryName,
                    TotalEnrollments = g.Count(),
                    Completed = g.Count(e => e.EnrollmentStatus.Status == "Completed"),
                    Dropped = g.Count(e => e.EnrollmentStatus.Status == "Dropped"),
                    Active = g.Count(e => e.EnrollmentStatus.Status != "Completed" && e.EnrollmentStatus.Status != "Dropped"),
                    CompletionRate = g.Count() == 0
                        ? 0
                        : Math.Round((double)g.Count(e => e.EnrollmentStatus.Status == "Completed") / g.Count() * 100, 1)
                })
                .OrderByDescending(x => x.TotalEnrollments)
                .ToListAsync();

            var statusCounts = await _context.Enrollments
                .AsNoTracking()
                .GroupBy(e => e.EnrollmentStatus.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            var totalEnrollments = statusCounts.Sum(x => x.Count);
            var totalCompleted = statusCounts.FirstOrDefault(x => x.Status == "Completed")?.Count ?? 0;
            var totalDropped = statusCounts.FirstOrDefault(x => x.Status == "Dropped")?.Count ?? 0;

            var result = new EnrollmentStatsReportDto
            {
                Summary = new EnrollmentSummaryDto
                {
                    TotalEnrollments = totalEnrollments,
                    TotalCompleted = totalCompleted,
                    TotalDropped = totalDropped,
                    TotalActive = totalEnrollments - totalCompleted - totalDropped,
                    OverallCompletionRate = totalEnrollments == 0
                        ? 0
                        : Math.Round((double)totalCompleted / totalEnrollments * 100, 1)
                },
                ByCourse = byCourse.Select(x => new EnrollmentByCourseDto
                {
                    CourseId = x.CourseId,
                    CourseTitle = x.CourseTitle,
                    CourseCode = x.CourseCode,
                    CategoryName = x.CategoryName,
                    TotalEnrollments = x.TotalEnrollments,
                    Completed = x.Completed,
                    Dropped = x.Dropped,
                    Active = x.Active,
                    CompletionRate = x.CompletionRate
                }).ToList()
            };

            return Ok(result);
        }

        // GET /api/reports/instructors
        [HttpGet("instructors")]
        [ProducesResponseType(typeof(InstructorStatsReportDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<InstructorStatsReportDto>> GetInstructorStats()
        {
            var stats = await _context.Instructors
                .AsNoTracking()
                .Select(i => new
                {
                    InstructorId = i.InstructorId,
                    Name = i.User.UserName,
                    Email = i.User.Email,
                    TotalSessions = i.CourseSessions.Count(),
                    CompletedSessions = i.CourseSessions.Count(s => s.Status.Status == "Completed"),
                    TotalTrainees = i.CourseSessions.SelectMany(s => s.Enrollments).Count(),
                    TotalAssessments = i.Assessments.Count(),
                    PassedAssessments = i.Assessments.Count(a => a.Result == "Pass"),
                    AvgPassRate = i.Assessments.Count() == 0
                        ? 0
                        : Math.Round((double)i.Assessments.Count(a => a.Result == "Pass") / i.Assessments.Count() * 100, 1)
                })
                .OrderByDescending(x => x.TotalSessions)
                .ToListAsync();

            var result = new InstructorStatsReportDto
            {
                Summary = new InstructorSummaryDto
                {
                    TotalInstructors = stats.Count,
                    AvgSessionsPerInstructor = stats.Count == 0
                        ? 0
                        : Math.Round(stats.Average(x => x.TotalSessions), 1),
                    AvgPassRate = stats.Count == 0
                        ? 0
                        : Math.Round(stats.Average(x => x.AvgPassRate), 1)
                },
                ByInstructor = stats.Select(x => new InstructorDataDto
                {
                    InstructorId = x.InstructorId,
                    Name = x.Name ?? string.Empty,
                    Email = x.Email ?? string.Empty,
                    TotalSessions = x.TotalSessions,
                    CompletedSessions = x.CompletedSessions,
                    TotalTrainees = x.TotalTrainees,
                    TotalAssessments = x.TotalAssessments,
                    PassedAssessments = x.PassedAssessments,
                    AvgPassRate = x.AvgPassRate
                }).ToList()
            };

            return Ok(result);
        }

        // GET /api/reports/certifications
        [HttpGet("certifications")]
        [ProducesResponseType(typeof(CertificationStatsReportDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CertificationStatsReportDto>> GetCertificationStats()
        {
            var stats = await _context.CertificationTracks
                .AsNoTracking()
                .Select(ct => new
                {
                    TrackId = ct.CertificationTrackId,
                    TrackName = ct.Name,
                    Description = ct.Description,
                    RequiredCoursesCount = ct.CertificationRequiredCourses.Count(),
                    TotalEnrolled = ct.TraineeCertifications.Count(),
                    Issued = ct.TraineeCertifications.Count(tc => tc.Status.Status == "Issued"),
                    Eligible = ct.TraineeCertifications.Count(tc => tc.Status.Status == "Eligible"),
                    InProgress = ct.TraineeCertifications.Count(tc => tc.Status.Status == "In Progress"),
                    Expired = ct.TraineeCertifications.Count(tc => tc.Status.Status == "Expired"),
                    CompletionRate = ct.TraineeCertifications.Count() == 0
                        ? 0
                        : Math.Round((double)ct.TraineeCertifications.Count(tc => tc.Status.Status == "Issued") /
                                     ct.TraineeCertifications.Count() * 100, 1)
                })
                .OrderByDescending(x => x.TotalEnrolled)
                .ToListAsync();

            var totalIssued = await _context.TraineeCertifications
                .AsNoTracking()
                .CountAsync(tc => tc.Status.Status == "Issued");

            var result = new CertificationStatsReportDto
            {
                Summary = new CertificationSummaryDto
                {
                    TotalTracks = stats.Count,
                    TotalIssued = totalIssued,
                    TotalInProgress = stats.Sum(x => x.InProgress),
                    TotalExpired = stats.Sum(x => x.Expired)
                },
                ByTrack = stats.Select(x => new CertificationByTrackDto
                {
                    TrackId = x.TrackId,
                    TrackName = x.TrackName,
                    Description = x.Description,
                    RequiredCoursesCount = x.RequiredCoursesCount,
                    TotalEnrolled = x.TotalEnrolled,
                    Issued = x.Issued,
                    Eligible = x.Eligible,
                    InProgress = x.InProgress,
                    Expired = x.Expired,
                    CompletionRate = x.CompletionRate
                }).ToList()
            };

            return Ok(result);
        }

        // GET /api/reports/revenue
        [HttpGet("revenue")]
        [ProducesResponseType(typeof(RevenueStatsReportDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<RevenueStatsReportDto>> GetRevenueStats()
        {
            var records = await _context.PaymentRecords
                .AsNoTracking()
                .Select(pr => new
                {
                    pr.TotalAmount,
                    Status = pr.Status.Status,
                    IssuedYear = pr.IssuedDate.Year,
                    IssuedMonth = pr.IssuedDate.Month,
                    CourseId = pr.Enrollment.Session.CourseId,
                    CourseTitle = pr.Enrollment.Session.Course.Title,
                    CourseCode = pr.Enrollment.Session.Course.CourseCode,
                    Collected = pr.PaymentTransactions.Sum(pt => pt.Amount)
                })
                .ToListAsync();

            var totalRevenue = records.Sum(pr => pr.TotalAmount);
            var totalCollected = records.Sum(pr => pr.Collected);
            var totalOutstanding = totalRevenue - totalCollected;
            var overdueCount = records.Count(pr => pr.Status == "Overdue");

            var byCourse = records
                .GroupBy(pr => new { pr.CourseId, pr.CourseTitle, pr.CourseCode })
                .Select(g => new
                {
                    g.Key.CourseId,
                    g.Key.CourseTitle,
                    g.Key.CourseCode,
                    TotalInvoiced = g.Sum(pr => pr.TotalAmount),
                    TotalCollected = g.Sum(pr => pr.Collected),
                    Outstanding = g.Sum(pr => pr.TotalAmount) - g.Sum(pr => pr.Collected),
                    OverdueCount = g.Count(pr => pr.Status == "Overdue")
                })
                .OrderByDescending(x => x.TotalInvoiced)
                .ToList();

            var byMonth = records
                .GroupBy(pr => new { Year = pr.IssuedYear, Month = pr.IssuedMonth })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                    TotalInvoiced = g.Sum(pr => pr.TotalAmount),
                    TotalCollected = g.Sum(pr => pr.Collected)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            var result = new RevenueStatsReportDto
            {
                Summary = new RevenueSummaryDto
                {
                    TotalRevenue = totalRevenue,
                    TotalCollected = totalCollected,
                    TotalOutstanding = totalOutstanding,
                    OverdueCount = overdueCount,
                    CollectionRate = totalRevenue == 0
                        ? 0
                        : Math.Round((double)totalCollected / (double)totalRevenue * 100, 1)
                },
                ByCourse = byCourse.Select(x => new RevenueByCourseDto
                {
                    CourseId = x.CourseId,
                    CourseTitle = x.CourseTitle,
                    CourseCode = x.CourseCode,
                    TotalInvoiced = x.TotalInvoiced,
                    TotalCollected = x.TotalCollected,
                    Outstanding = x.Outstanding,
                    OverdueCount = x.OverdueCount
                }).ToList(),
                ByMonth = byMonth.Select(x => new RevenueByMonthDto
                {
                    Year = x.Year,
                    Month = x.Month,
                    MonthName = x.MonthName,
                    TotalInvoiced = x.TotalInvoiced,
                    TotalCollected = x.TotalCollected
                }).ToList()
            };

            return Ok(result);
        }

        // GET /api/reports/overview
        [HttpGet("overview")]
        [ProducesResponseType(typeof(OverviewReportDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<OverviewReportDto>> GetOverview()
        {
            var totalTrainees = await _context.Trainees.AsNoTracking().CountAsync();
            var totalInstructors = await _context.Instructors.AsNoTracking().CountAsync();
            var totalCourses = await _context.Courses.AsNoTracking().CountAsync(c => c.IsActive);
            var totalSessions = await _context.CourseSessions.AsNoTracking().CountAsync();

            var enrollmentCounts = await _context.Enrollments
                .AsNoTracking()
                .GroupBy(e => e.EnrollmentStatus.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            var totalEnrollments = enrollmentCounts.Sum(x => x.Count);
            var completedEnrollments = enrollmentCounts.FirstOrDefault(x => x.Status == "Completed")?.Count ?? 0;

            var totalCertificationsIssued = await _context.TraineeCertifications
                .AsNoTracking()
                .CountAsync(tc => tc.Status.Status == "Issued");

            var totalRevenue = await _context.PaymentRecords
                .AsNoTracking()
                .SumAsync(pr => (decimal?)pr.TotalAmount) ?? 0m;

            var totalCollected = await _context.PaymentTransactions
                .AsNoTracking()
                .SumAsync(pt => (decimal?)pt.Amount) ?? 0m;

            var assessmentCounts = await _context.Assessments
                .AsNoTracking()
                .GroupBy(a => a.Result)
                .Select(g => new { Result = g.Key, Count = g.Count() })
                .ToListAsync();

            var totalAssessments = assessmentCounts.Sum(x => x.Count);
            var passedAssessments = assessmentCounts.FirstOrDefault(x => x.Result == "Pass")?.Count ?? 0;
            var passRate = totalAssessments == 0
                ? 0
                : Math.Round((double)passedAssessments / totalAssessments * 100, 1);

            var result = new OverviewReportDto
            {
                TotalTrainees = totalTrainees,
                TotalInstructors = totalInstructors,
                TotalCourses = totalCourses,
                TotalSessions = totalSessions,
                TotalEnrollments = totalEnrollments,
                CompletedEnrollments = completedEnrollments,
                TotalCertificationsIssued = totalCertificationsIssued,
                TotalRevenue = totalRevenue,
                TotalCollected = totalCollected,
                TotalOutstanding = totalRevenue - totalCollected,
                OverallPassRate = passRate,
                CompletionRate = totalEnrollments == 0
                    ? 0
                    : Math.Round((double)completedEnrollments / totalEnrollments * 100, 1)
            };

            return Ok(result);
        }
    }
}
