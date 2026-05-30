using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Frontend.Helpers;
using MVC_Frontend.Models;
using Web_API.Models;

namespace MVC_Frontend.Controllers
{
    [Authorize]
    public class CertificationController : Controller
    {
        private readonly TrainingInstituteDBContext _context;

        public CertificationController(TrainingInstituteDBContext context)
        {
            _context = context;
        }

        // GET: Certification — list all tracks
        public async Task<IActionResult> Index()
        {
            var query = _context.CertificationTracks
                .Include(t => t.CertificationRequiredCourses)
                .Include(t => t.TraineeCertifications)
                .AsQueryable();

            if (!User.IsInRole(AppRoles.Coordinator))
                query = query.Where(t => t.IsActive);

            var tracks = await query.OrderBy(t => t.Name).ToListAsync();
            return View(tracks);
        }

        // GET: Certification/Details/5 — coordinator: track info, required courses, certifications
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> Details(int id)
        {
            var track = await _context.CertificationTracks
                .Include(t => t.CertificationRequiredCourses.OrderBy(rc => rc.SequenceOrder ?? 999))
                    .ThenInclude(rc => rc.Course)
                .Include(t => t.TraineeCertifications)
                    .ThenInclude(tc => tc.Trainee)
                .Include(t => t.TraineeCertifications)
                    .ThenInclude(tc => tc.Status)
                .FirstOrDefaultAsync(t => t.CertificationTrackId == id);

            if (track == null)
                return NotFound();

            int totalMandatory = track.CertificationRequiredCourses.Count(rc => rc.IsMandatory);
            var mandatoryIdsList = track.CertificationRequiredCourses
                .Where(rc => rc.IsMandatory)
                .Select(rc => rc.CourseId)
                .ToList();

            var summaries = new List<TrackEnrollmentSummaryViewModel>();
            foreach (var cert in track.TraineeCertifications)
            {
                // Pull completions to memory to avoid EF Core 9 CTE generation
                var allCompleted = await _context.TraineeCourseCompletions
                    .Where(c => c.TraineeId == cert.TraineeId && c.Result == "Pass")
                    .Select(c => c.CourseId)
                    .ToListAsync();
                var completedCount = allCompleted.Intersect(mandatoryIdsList).Count();

                var user = await _context.Users.FindAsync(cert.Trainee.UserId);
                summaries.Add(new TrackEnrollmentSummaryViewModel
                {
                    TraineeId = cert.TraineeId,
                    TraineeName = user?.Email ?? $"Trainee {cert.TraineeId}",
                    CompletedMandatory = completedCount,
                    TotalMandatory = totalMandatory,
                    Certification = cert
                });
            }

            ViewBag.Summaries = summaries;
            return View(track);
        }

        // GET: Certification/Create
        [Authorize(Roles = AppRoles.Coordinator)]
        public IActionResult Create() =>
            View(new CertificationTrackFormViewModel { IsActive = true });

        // POST: Certification/Create
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> Create(CertificationTrackFormViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            if (await _context.CertificationTracks.AnyAsync(t => t.Name == vm.Name.Trim()))
            {
                ModelState.AddModelError(nameof(vm.Name), "A certification track with this name already exists.");
                return View(vm);
            }

            _context.CertificationTracks.Add(new CertificationTrack
            {
                Name = vm.Name.Trim(),
                Description = vm.Description?.Trim(),
                ValidityPeriod = vm.ValidityPeriod,
                IsActive = vm.IsActive,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Certification track \"{vm.Name}\" created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Certification/Edit/5
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> Edit(int id)
        {
            var track = await _context.CertificationTracks.FindAsync(id);
            if (track == null)
                return NotFound();

            return View(new CertificationTrackFormViewModel
            {
                CertificationTrackId = track.CertificationTrackId,
                Name = track.Name,
                Description = track.Description,
                ValidityPeriod = track.ValidityPeriod,
                IsActive = track.IsActive
            });
        }

        // POST: Certification/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> Edit(int id, CertificationTrackFormViewModel vm)
        {
            if (id != vm.CertificationTrackId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(vm);

            var track = await _context.CertificationTracks.FindAsync(id);
            if (track == null)
                return NotFound();

            if (await _context.CertificationTracks.AnyAsync(t =>
                t.Name == vm.Name.Trim() && t.CertificationTrackId != id))
            {
                ModelState.AddModelError(nameof(vm.Name), "A certification track with this name already exists.");
                return View(vm);
            }

            track.Name = vm.Name.Trim();
            track.Description = vm.Description?.Trim();
            track.ValidityPeriod = vm.ValidityPeriod;
            track.IsActive = vm.IsActive;
            track.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Certification track \"{track.Name}\" updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Certification/Deactivate/5
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> Deactivate(int id)
        {
            var track = await _context.CertificationTracks
                .Include(t => t.CertificationRequiredCourses)
                .Include(t => t.TraineeCertifications)
                .FirstOrDefaultAsync(t => t.CertificationTrackId == id);

            if (track == null)
                return NotFound();

            return View(track);
        }

        // POST: Certification/Deactivate/5
        [HttpPost, ActionName("Deactivate"), ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> DeactivateConfirmed(int id)
        {
            var track = await _context.CertificationTracks.FindAsync(id);
            if (track == null)
                return NotFound();

            track.IsActive = false;
            track.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Certification track \"{track.Name}\" has been deactivated.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Certification/AddCourse/5  (id = trackId)
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> AddCourse(int id)
        {
            var track = await _context.CertificationTracks.FindAsync(id);
            if (track == null)
                return NotFound();

            var existingCourseIds = await _context.CertificationRequiredCourses
                .Where(rc => rc.CertificationTrackId == id)
                .Select(rc => rc.CourseId)
                .ToListAsync();

            // Pull to memory first to avoid NOT IN generating a CTE in EF Core 9
            var allActiveCourses = await _context.Courses
                .Where(c => c.IsActive)
                .OrderBy(c => c.Title)
                .ToListAsync();
            var availableCourses = allActiveCourses
                .Where(c => !existingCourseIds.Contains(c.CourseId))
                .Select(c => new SelectListItem
                {
                    Value = c.CourseId.ToString(),
                    Text = $"{c.CourseCode} – {c.Title}"
                })
                .ToList();

            return View(new AddCourseToTrackViewModel
            {
                CertificationTrackId = id,
                TrackName = track.Name,
                IsMandatory = true,
                AvailableCourses = availableCourses
            });
        }

        // POST: Certification/AddCourse
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> AddCourse(AddCourseToTrackViewModel vm)
        {
            if (!ModelState.IsValid || vm.CourseId == null)
            {
                var existingIds = await _context.CertificationRequiredCourses
                    .Where(rc => rc.CertificationTrackId == vm.CertificationTrackId)
                    .Select(rc => rc.CourseId)
                    .ToListAsync();

                var allActive = await _context.Courses
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Title)
                    .ToListAsync();
                vm.AvailableCourses = allActive
                    .Where(c => !existingIds.Contains(c.CourseId))
                    .Select(c => new SelectListItem
                    {
                        Value = c.CourseId.ToString(),
                        Text = $"{c.CourseCode} – {c.Title}"
                    })
                    .ToList();

                var t = await _context.CertificationTracks.FindAsync(vm.CertificationTrackId);
                vm.TrackName = t?.Name ?? string.Empty;
                return View(vm);
            }

            _context.CertificationRequiredCourses.Add(new CertificationRequiredCourse
            {
                CertificationTrackId = vm.CertificationTrackId,
                CourseId = vm.CourseId.Value,
                SequenceOrder = vm.SequenceOrder,
                IsMandatory = vm.IsMandatory
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = "Course added to certification track.";
            return RedirectToAction(nameof(Details), new { id = vm.CertificationTrackId });
        }

        // POST: Certification/RemoveCourse
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> RemoveCourse(int reqCourseId, int trackId)
        {
            var req = await _context.CertificationRequiredCourses.FindAsync(reqCourseId);
            if (req == null)
                return NotFound();

            _context.CertificationRequiredCourses.Remove(req);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Course removed from certification track.";
            return RedirectToAction(nameof(Details), new { id = trackId });
        }

        // GET: Certification/MyProgress — trainee overview of all tracks
        [Authorize(Roles = AppRoles.Trainee)]
        public async Task<IActionResult> MyProgress()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.UserId == userId);
            if (trainee == null)
                return NotFound();

            var tracks = await _context.CertificationTracks
                .Where(t => t.IsActive)
                .Include(t => t.CertificationRequiredCourses)
                .OrderBy(t => t.Name)
                .ToListAsync();

            var passedCourseIds = (await _context.TraineeCourseCompletions
                .Where(c => c.TraineeId == trainee.TraineeId && c.Result == "Pass")
                .Select(c => c.CourseId)
                .ToListAsync())
                .Distinct()
                .ToList();

            var myCerts = await _context.TraineeCertifications
                .Include(tc => tc.Status)
                .Where(tc => tc.TraineeId == trainee.TraineeId)
                .ToDictionaryAsync(tc => tc.CertificationTrackId);

            var progressList = tracks.Select(track =>
            {
                var mandatory = track.CertificationRequiredCourses.Where(rc => rc.IsMandatory).ToList();
                int completed = mandatory.Count(rc => passedCourseIds.Contains(rc.CourseId));
                myCerts.TryGetValue(track.CertificationTrackId, out var cert);

                return new CertificationProgressViewModel
                {
                    Track = track,
                    TraineeCert = cert,
                    TotalMandatory = mandatory.Count,
                    CompletedMandatory = completed,
                    TraineeId = trainee.TraineeId
                };
            }).ToList();

            return View(progressList);
        }

        // GET: Certification/Progress/5 — detailed course-by-course progress
        // Trainee: own progress. Coordinator: ?traineeId=X
        public async Task<IActionResult> Progress(int id, int? traineeId)
        {
            var track = await _context.CertificationTracks
                .Include(t => t.CertificationRequiredCourses.OrderBy(rc => rc.SequenceOrder ?? 999))
                    .ThenInclude(rc => rc.Course)
                .FirstOrDefaultAsync(t => t.CertificationTrackId == id);

            if (track == null)
                return NotFound();

            int resolvedTraineeId;
            string? traineeName = null;

            if (User.IsInRole(AppRoles.Coordinator))
            {
                if (traineeId == null)
                    return BadRequest("traineeId is required.");

                var trainee = await _context.Trainees.FindAsync(traineeId.Value);
                if (trainee == null)
                    return NotFound();

                resolvedTraineeId = traineeId.Value;
                var user = await _context.Users.FindAsync(trainee.UserId);
                traineeName = user?.Email ?? $"Trainee {resolvedTraineeId}";
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.UserId == userId);
                if (trainee == null)
                    return NotFound();

                resolvedTraineeId = trainee.TraineeId;
            }

            var courseIds = track.CertificationRequiredCourses.Select(rc => rc.CourseId).ToList();

            // Filter by traineeId only in SQL, then filter by courseIds in memory
            var allTraineeCompletions = await _context.TraineeCourseCompletions
                .Where(c => c.TraineeId == resolvedTraineeId)
                .ToListAsync();
            var completions = allTraineeCompletions
                .Where(c => courseIds.Contains(c.CourseId))
                .ToList();

            var completionMap = completions
                .GroupBy(c => c.CourseId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(c => c.CompletionDate).First());

            var courseItems = track.CertificationRequiredCourses
                .Select(rc =>
                {
                    completionMap.TryGetValue(rc.CourseId, out var comp);
                    return new CourseProgressItem
                    {
                        CourseId = rc.CourseId,
                        CourseCode = rc.Course.CourseCode,
                        CourseTitle = rc.Course.Title,
                        IsMandatory = rc.IsMandatory,
                        SequenceOrder = rc.SequenceOrder,
                        IsCompleted = comp?.Result == "Pass",
                        Result = comp?.Result,
                        CompletionDate = comp?.CompletionDate
                    };
                })
                .ToList();

            var cert = await _context.TraineeCertifications
                .Include(tc => tc.Status)
                .FirstOrDefaultAsync(tc =>
                    tc.TraineeId == resolvedTraineeId &&
                    tc.CertificationTrackId == id);

            int totalMandatory = courseItems.Count(c => c.IsMandatory);
            int completedMandatory = courseItems.Count(c => c.IsMandatory && c.IsCompleted);

            return View(new CertificationProgressViewModel
            {
                Track = track,
                TraineeCert = cert,
                Courses = courseItems,
                TotalMandatory = totalMandatory,
                CompletedMandatory = completedMandatory,
                TraineeName = traineeName,
                TraineeId = resolvedTraineeId
            });
        }

        // POST: Certification/IssueCertificate
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Coordinator)]
        public async Task<IActionResult> IssueCertificate(int traineeId, int trackId)
        {
            var track = await _context.CertificationTracks.FindAsync(trackId);
            if (track == null)
                return NotFound();

            var mandatoryIds = await _context.CertificationRequiredCourses
                .Where(rc => rc.CertificationTrackId == trackId && rc.IsMandatory)
                .Select(rc => rc.CourseId)
                .ToListAsync();

            var allPassed = await _context.TraineeCourseCompletions
                .Where(c => c.TraineeId == traineeId && c.Result == "Pass")
                .Select(c => c.CourseId)
                .ToListAsync();
            var passedCount = allPassed.Intersect(mandatoryIds).Count();

            if (passedCount < mandatoryIds.Count)
            {
                TempData["Error"] = "Trainee has not completed all mandatory courses. Cannot issue certificate.";
                return RedirectToAction(nameof(Progress), new { id = trackId, traineeId });
            }

            var issuedStatus = await _context.CertificationStatuses.FirstAsync(s => s.Status == "Issued");

            var cert = await _context.TraineeCertifications
                .FirstOrDefaultAsync(tc =>
                    tc.TraineeId == traineeId &&
                    tc.CertificationTrackId == trackId);

            var now = DateTime.Now;
            var certNumber = $"CERT-{trackId:D3}-{traineeId:D4}-{now:yyyyMMdd}";

            if (cert == null)
            {
                cert = new TraineeCertification
                {
                    TraineeId = traineeId,
                    CertificationTrackId = trackId,
                    StatusId = issuedStatus.StatusId,
                    EligibleDate = now,
                    CertificateIssuedDate = now,
                    CertificateNumber = certNumber,
                    ExpiryDate = track.ValidityPeriod.HasValue
                        ? now.AddMonths(track.ValidityPeriod.Value) : null
                };
                _context.TraineeCertifications.Add(cert);
            }
            else
            {
                cert.StatusId = issuedStatus.StatusId;
                cert.CertificateIssuedDate = now;
                cert.CertificateNumber ??= certNumber;
                cert.EligibleDate ??= now;
                cert.ExpiryDate = track.ValidityPeriod.HasValue
                    ? now.AddMonths(track.ValidityPeriod.Value) : null;
            }

            await _context.SaveChangesAsync();

            // Notify the trainee
            var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.TraineeId == traineeId);
            if (trainee != null)
            {
                await NotificationHelper.CreateAsync(_context, trainee.UserId,
                    "Certificate Issued",
                    $"Congratulations! Your certificate for \"{track.Name}\" has been issued. Certificate number: {cert.CertificateNumber}.",
                    "Certification", "TraineeCertification");
            }

            TempData["Success"] = $"Certificate {cert.CertificateNumber} issued successfully.";
            return RedirectToAction(nameof(Details), new { id = trackId });
        }

        // GET: Certification/MyCertificates — trainee views earned certificates
        [Authorize(Roles = AppRoles.Trainee)]
        public async Task<IActionResult> MyCertificates()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.UserId == userId);
            if (trainee == null)
                return NotFound();

            var certs = await _context.TraineeCertifications
                .Include(tc => tc.CertificationTrack)
                .Include(tc => tc.Status)
                .Where(tc => tc.TraineeId == trainee.TraineeId)
                .OrderByDescending(tc => tc.CertificateIssuedDate)
                .ToListAsync();

            return View(certs);
        }
    }
}
