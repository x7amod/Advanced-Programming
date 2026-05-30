using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Frontend.Models;
using Web_API.Models;

namespace MVC_Frontend.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly TrainingInstituteDBContext _context;

        public HomeController(TrainingInstituteDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(userId);
            var userName = user?.UserName ?? "User";

            var vm = new DashboardViewModel { UserName = userName };

            if (User.IsInRole(AppRoles.Trainee))
            {
                vm.Role = AppRoles.Trainee;
                vm.Trainee = await BuildTraineeDashboard(userId!);
            }
            else if (User.IsInRole(AppRoles.Instructor))
            {
                vm.Role = AppRoles.Instructor;
                vm.Instructor = await BuildInstructorDashboard(userId!);
            }
            else if (User.IsInRole(AppRoles.Coordinator))
            {
                vm.Role = AppRoles.Coordinator;
                vm.Coordinator = await BuildCoordinatorDashboard();
            }

            return View(vm);
        }

        // ── Trainee ──────────────────────────────────────────────────────────────

        private async Task<TraineeDashboardViewModel> BuildTraineeDashboard(string userId)
        {
            var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.UserId == userId);
            if (trainee == null) return new TraineeDashboardViewModel();

            var enrollments = await _context.Enrollments
                .Include(e => e.EnrollmentStatus)
                .Include(e => e.Session).ThenInclude(s => s.Course)
                .Include(e => e.Session).ThenInclude(s => s.Classroom)
                .Where(e => e.TraineeId == trainee.TraineeId)
                .AsSplitQuery()
                .ToListAsync();

            var statusGroups = enrollments
                .GroupBy(e => e.EnrollmentStatus.Status)
                .ToDictionary(g => g.Key, g => g.Count());

            var today = DateTime.Today;
            var upcoming = enrollments
                .Where(e => e.Session.SessionDate >= today &&
                            e.EnrollmentStatus.Status != "Dropped")
                .OrderBy(e => e.Session.SessionDate)
                .Take(5)
                .Select(e => new DashUpcomingSessionViewModel
                {
                    SessionId = e.SessionId,
                    CourseTitle = e.Session.Course.Title,
                    SessionDate = e.Session.SessionDate,
                    StartTime = e.Session.StartTime,
                    ClassroomName = e.Session.Classroom?.Name
                }).ToList();

            // Certification progress
            var tracks = await _context.CertificationTracks
                .Include(t => t.CertificationRequiredCourses)
                .Include(t => t.TraineeCertifications.Where(tc => tc.TraineeId == trainee.TraineeId))
                    .ThenInclude(tc => tc.Status)
                .Where(t => t.IsActive)
                .AsSplitQuery()
                .ToListAsync();

            var passedCourseIds = await _context.TraineeCourseCompletions
                .Where(c => c.TraineeId == trainee.TraineeId && c.Result == "Pass")
                .Select(c => c.CourseId)
                .ToListAsync();

            var certProgress = tracks.Select(t =>
            {
                var mandatory = t.CertificationRequiredCourses.Where(rc => rc.IsMandatory).Select(rc => rc.CourseId).ToList();
                var done = mandatory.Intersect(passedCourseIds).Count();
                var myCert = t.TraineeCertifications.FirstOrDefault();
                return new DashCertProgressViewModel
                {
                    TrackName = t.Name,
                    Completed = done,
                    Total = mandatory.Count,
                    Status = myCert?.Status?.Status ?? (done >= mandatory.Count && mandatory.Count > 0 ? "Eligible" : "In Progress")
                };
            }).ToList();

            // Payments
            var payments = await _context.PaymentRecords
                .Include(p => p.PaymentTransactions)
                .Where(p => p.Enrollment.TraineeId == trainee.TraineeId)
                .AsSplitQuery()
                .ToListAsync();

            var labels = statusGroups.Keys.ToList();
            var counts = statusGroups.Values.ToList();

            return new TraineeDashboardViewModel
            {
                TotalEnrollments = enrollments.Count,
                ActiveEnrollments = statusGroups.TryGetValue("Enrolled", out var e1) ? e1 : 0
                                  + (statusGroups.TryGetValue("Confirmed", out var e2) ? e2 : 0)
                                  + (statusGroups.TryGetValue("Attending", out var e3) ? e3 : 0),
                CompletedEnrollments = statusGroups.TryGetValue("Completed", out var ec) ? ec : 0,
                DroppedEnrollments = statusGroups.TryGetValue("Dropped", out var ed) ? ed : 0,
                UpcomingSessions = upcoming,
                CertProgress = certProgress,
                TotalOwed = payments.Sum(p => p.TotalAmount),
                TotalPaid = payments.Sum(p => p.PaymentTransactions.Sum(t => t.Amount)),
                EnrollmentLabels = labels,
                EnrollmentCounts = counts
            };
        }

        // ── Instructor ───────────────────────────────────────────────────────────

        private async Task<InstructorDashboardViewModel> BuildInstructorDashboard(string userId)
        {
            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == userId);
            if (instructor == null) return new InstructorDashboardViewModel();

            var sessions = await _context.CourseSessions
                .Include(s => s.Course)
                .Include(s => s.Status)
                .Include(s => s.Classroom)
                .Where(s => s.InstructorId == instructor.InstructorId)
                .AsSplitQuery()
                .ToListAsync();

            var today = DateTime.Today;
            var upcoming = sessions
                .Where(s => s.SessionDate >= today && s.Status.Status == "Scheduled")
                .OrderBy(s => s.SessionDate)
                .Take(5)
                .Select(s => new DashUpcomingSessionViewModel
                {
                    SessionId = s.SessionId,
                    CourseTitle = s.Course.Title,
                    SessionDate = s.SessionDate,
                    StartTime = s.StartTime,
                    ClassroomName = s.Classroom?.Name
                }).ToList();

            var assessments = await _context.Assessments
                .Where(a => a.InstructorId == instructor.InstructorId)
                .ToListAsync();

            // Monthly sessions — last 6 months
            var monthLabels = new List<string>();
            var monthCounts = new List<int>();
            for (int i = 5; i >= 0; i--)
            {
                var month = DateTime.Today.AddMonths(-i);
                monthLabels.Add(month.ToString("MMM yyyy"));
                monthCounts.Add(sessions.Count(s => s.SessionDate.Year == month.Year && s.SessionDate.Month == month.Month));
            }

            return new InstructorDashboardViewModel
            {
                UpcomingSessionCount = upcoming.Count,
                CompletedSessionCount = sessions.Count(s => s.Status.Status == "Completed"),
                TotalTraineesAssessed = assessments.Count,
                PassCount = assessments.Count(a => a.Result == "Pass"),
                FailCount = assessments.Count(a => a.Result == "Fail"),
                UpcomingSessions = upcoming,
                MonthLabels = monthLabels,
                MonthSessionCounts = monthCounts
            };
        }

        // ── Coordinator ──────────────────────────────────────────────────────────

        private async Task<CoordinatorDashboardViewModel> BuildCoordinatorDashboard()
        {
            var now = DateTime.Now;

            var traineeCount = await _context.Trainees.CountAsync();
            var instructorCount = await _context.Instructors.CountAsync();
            var activeCourseCount = await _context.Courses.CountAsync(c => c.IsActive);
            var sessionsThisMonth = await _context.CourseSessions
                .CountAsync(s => s.SessionDate.Year == now.Year && s.SessionDate.Month == now.Month);

            // Enrollment status breakdown
            var enrollmentGroups = await _context.Enrollments
                .Include(e => e.EnrollmentStatus)
                .GroupBy(e => e.EnrollmentStatus.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            // Revenue
            var allRecords = await _context.PaymentRecords
                .Include(p => p.PaymentTransactions)
                .AsSplitQuery()
                .ToListAsync();
            var totalInvoiced = allRecords.Sum(p => p.TotalAmount);
            var totalCollected = allRecords.Sum(p => p.PaymentTransactions.Sum(t => t.Amount));

            // Monthly enrollments — last 6 months
            var allEnrollments = await _context.Enrollments.Select(e => e.EnrollmentDate).ToListAsync();
            var monthLabels = new List<string>();
            var monthCounts = new List<int>();
            for (int i = 5; i >= 0; i--)
            {
                var month = DateTime.Today.AddMonths(-i);
                monthLabels.Add(month.ToString("MMM yyyy"));
                monthCounts.Add(allEnrollments.Count(d => d.Year == month.Year && d.Month == month.Month));
            }

            // Top 5 courses by enrollment
            var topCourses = await _context.Enrollments
                .Include(e => e.Session).ThenInclude(s => s.Course)
                .Where(e => e.EnrollmentStatus.Status != "Dropped")
                .GroupBy(e => e.Session.Course.Title)
                .Select(g => new DashTopCourseViewModel { CourseTitle = g.Key, EnrollmentCount = g.Count() })
                .OrderByDescending(x => x.EnrollmentCount)
                .Take(5)
                .ToListAsync();

            return new CoordinatorDashboardViewModel
            {
                TotalTrainees = traineeCount,
                TotalInstructors = instructorCount,
                TotalActiveCourses = activeCourseCount,
                SessionsThisMonth = sessionsThisMonth,
                TotalInvoiced = totalInvoiced,
                TotalCollected = totalCollected,
                EnrollmentLabels = enrollmentGroups.Select(g => g.Status).ToList(),
                EnrollmentCounts = enrollmentGroups.Select(g => g.Count).ToList(),
                MonthLabels = monthLabels,
                MonthEnrollmentCounts = monthCounts,
                TopCourses = topCourses
            };
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
