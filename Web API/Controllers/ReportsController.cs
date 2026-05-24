using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_API.Models;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Training Coordinator")]
    public class ReportsController : ControllerBase
    {
        private readonly TrainingInstituteDBContext _context;

        public ReportsController(TrainingInstituteDBContext context)
        {
            _context = context;
        }

        // GET /api/reports/enrollments
        [HttpGet("enrollments")]
        public async Task<IActionResult> GetEnrollmentStats()
        {
            var stats = await _context.Enrollments
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                        .ThenInclude(c => c.Category)
                .Include(e => e.EnrollmentStatus)
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
                    CompletionRate = g.Count() == 0 ? 0 :
                        Math.Round((double)g.Count(e => e.EnrollmentStatus.Status == "Completed") / g.Count() * 100, 1)
                })
                .OrderByDescending(x => x.TotalEnrollments)
                .ToListAsync();

            var totalEnrollments = await _context.Enrollments.CountAsync();
            var totalCompleted = await _context.Enrollments
                .Include(e => e.EnrollmentStatus)
                .CountAsync(e => e.EnrollmentStatus.Status == "Completed");
            var totalDropped = await _context.Enrollments
                .Include(e => e.EnrollmentStatus)
                .CountAsync(e => e.EnrollmentStatus.Status == "Dropped");

            return Ok(new
            {
                summary = new
                {
                    totalEnrollments,
                    totalCompleted,
                    totalDropped,
                    totalActive = totalEnrollments - totalCompleted - totalDropped,
                    overallCompletionRate = totalEnrollments == 0 ? 0 :
                        Math.Round((double)totalCompleted / totalEnrollments * 100, 1)
                },
                byCourse = stats
            });
        }

        // GET /api/reports/instructors
        [HttpGet("instructors")]
        public async Task<IActionResult> GetInstructorStats()
        {
            var stats = await _context.Instructors
                .Include(i => i.User)
                .Include(i => i.CourseSessions)
                    .ThenInclude(s => s.Enrollments)
                        .ThenInclude(e => e.EnrollmentStatus)
                .Include(i => i.Assessments)
                .Select(i => new
                {
                    InstructorId = i.InstructorId,
                    Name = i.User.UserName,
                    Email = i.User.Email,
                    HireDate = i.HireDate,
                    TotalSessions = i.CourseSessions.Count(),
                    TotalTrainees = i.CourseSessions
                        .SelectMany(s => s.Enrollments).Count(),
                    CompletedSessions = i.CourseSessions
                        .Count(s => s.Status.Status == "Completed"),
                    TotalAssessments = i.Assessments.Count(),
                    PassedAssessments = i.Assessments
                        .Count(a => a.Result == "Pass"),
                    AvgPassRate = i.Assessments.Count() == 0 ? 0 :
                        Math.Round((double)i.Assessments.Count(a => a.Result == "Pass") /
                        i.Assessments.Count() * 100, 1)
                })
                .OrderByDescending(x => x.TotalSessions)
                .ToListAsync();

            return Ok(new
            {
                summary = new
                {
                    totalInstructors = stats.Count,
                    avgSessionsPerInstructor = stats.Count == 0 ? 0 :
                        Math.Round(stats.Average(x => x.TotalSessions), 1),
                    avgPassRate = stats.Count == 0 ? 0 :
                        Math.Round(stats.Average(x => x.AvgPassRate), 1)
                },
                byInstructor = stats
            });
        }

        // GET /api/reports/certifications
        [HttpGet("certifications")]
        public async Task<IActionResult> GetCertificationStats()
        {
            var stats = await _context.CertificationTracks
                .Include(ct => ct.TraineeCertifications)
                    .ThenInclude(tc => tc.Status)
                .Include(ct => ct.CertificationRequiredCourses)
                .Select(ct => new
                {
                    TrackId = ct.CertificationTrackId,
                    TrackName = ct.Name,
                    Description = ct.Description,
                    ValidityPeriod = ct.ValidityPeriod,
                    RequiredCoursesCount = ct.CertificationRequiredCourses.Count(),
                    TotalEnrolled = ct.TraineeCertifications.Count(),
                    Issued = ct.TraineeCertifications
                        .Count(tc => tc.Status.Status == "Issued"),
                    Eligible = ct.TraineeCertifications
                        .Count(tc => tc.Status.Status == "Eligible"),
                    InProgress = ct.TraineeCertifications
                        .Count(tc => tc.Status.Status == "In Progress"),
                    Expired = ct.TraineeCertifications
                        .Count(tc => tc.Status.Status == "Expired"),
                    CompletionRate = ct.TraineeCertifications.Count() == 0 ? 0 :
                        Math.Round((double)ct.TraineeCertifications
                        .Count(tc => tc.Status.Status == "Issued") /
                        ct.TraineeCertifications.Count() * 100, 1)
                })
                .OrderByDescending(x => x.TotalEnrolled)
                .ToListAsync();

            var totalIssued = await _context.TraineeCertifications
                .Include(tc => tc.Status)
                .CountAsync(tc => tc.Status.Status == "Issued");

            return Ok(new
            {
                summary = new
                {
                    totalTracks = stats.Count,
                    totalIssued,
                    totalInProgress = stats.Sum(x => x.InProgress),
                    totalExpired = stats.Sum(x => x.Expired)
                },
                byTrack = stats
            });
        }

        // GET /api/reports/revenue
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenueStats()
        {
            var records = await _context.PaymentRecords
                .Include(pr => pr.Status)
                .Include(pr => pr.Enrollment)
                    .ThenInclude(e => e.Session)
                        .ThenInclude(s => s.Course)
                .Include(pr => pr.PaymentTransactions)
                .ToListAsync();

            var totalRevenue = records.Sum(pr => pr.TotalAmount);
            var totalCollected = records
                .SelectMany(pr => pr.PaymentTransactions)
                .Sum(pt => pt.Amount);
            var totalOutstanding = totalRevenue - totalCollected;
            var overdueCount = records
                .Count(pr => pr.Status.Status == "Overdue");

            var byCourse = records
                .GroupBy(pr => new
                {
                    CourseId = pr.Enrollment.Session.CourseId,
                    CourseTitle = pr.Enrollment.Session.Course.Title,
                    CourseCode = pr.Enrollment.Session.Course.CourseCode
                })
                .Select(g => new
                {
                    g.Key.CourseId,
                    g.Key.CourseTitle,
                    g.Key.CourseCode,
                    TotalInvoiced = g.Sum(pr => pr.TotalAmount),
                    TotalCollected = g.SelectMany(pr => pr.PaymentTransactions)
                        .Sum(pt => pt.Amount),
                    Outstanding = g.Sum(pr => pr.TotalAmount) -
                        g.SelectMany(pr => pr.PaymentTransactions).Sum(pt => pt.Amount),
                    OverdueCount = g.Count(pr => pr.Status.Status == "Overdue")
                })
                .OrderByDescending(x => x.TotalInvoiced)
                .ToList();

            var byMonth = records
                .GroupBy(pr => new
                {
                    Year = pr.IssuedDate.Year,
                    Month = pr.IssuedDate.Month
                })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = new DateTime(g.Key.Year, g.Key.Month, 1)
                        .ToString("MMM yyyy"),
                    TotalInvoiced = g.Sum(pr => pr.TotalAmount),
                    TotalCollected = g.SelectMany(pr => pr.PaymentTransactions)
                        .Sum(pt => pt.Amount)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            return Ok(new
            {
                summary = new
                {
                    totalRevenue,
                    totalCollected,
                    totalOutstanding,
                    overdueCount,
                    collectionRate = totalRevenue == 0 ? 0 :
                        Math.Round((double)totalCollected / (double)totalRevenue * 100, 1)
                },
                byCourse,
                byMonth
            });
        }

        // GET /api/reports/overview
        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            var totalTrainees = await _context.Trainees.CountAsync();
            var totalInstructors = await _context.Instructors.CountAsync();
            var totalCourses = await _context.Courses.CountAsync(c => c.IsActive);
            var totalSessions = await _context.CourseSessions.CountAsync();

            var totalEnrollments = await _context.Enrollments.CountAsync();
            var completedEnrollments = await _context.Enrollments
                .Include(e => e.EnrollmentStatus)
                .CountAsync(e => e.EnrollmentStatus.Status == "Completed");

            var totalCertificationsIssued = await _context.TraineeCertifications
                .Include(tc => tc.Status)
                .CountAsync(tc => tc.Status.Status == "Issued");

            var totalRevenue = await _context.PaymentRecords.SumAsync(pr => pr.TotalAmount);
            var totalCollected = await _context.PaymentTransactions.SumAsync(pt => pt.Amount);

            var passRate = await _context.Assessments.CountAsync() == 0 ? 0 :
                Math.Round((double)await _context.Assessments.CountAsync(a => a.Result == "Pass") /
                await _context.Assessments.CountAsync() * 100, 1);

            return Ok(new
            {
                totalTrainees,
                totalInstructors,
                totalCourses,
                totalSessions,
                totalEnrollments,
                completedEnrollments,
                totalCertificationsIssued,
                totalRevenue,
                totalCollected,
                totalOutstanding = totalRevenue - totalCollected,
                overallPassRate = passRate,
                completionRate = totalEnrollments == 0 ? 0 :
                    Math.Round((double)completedEnrollments / totalEnrollments * 100, 1)
            });
        }
    }
}