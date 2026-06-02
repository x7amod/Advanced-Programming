using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_API.Models;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class PublicController : ControllerBase
    {
        private readonly TrainingInstituteDBContext _context;

        public PublicController(TrainingInstituteDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Public endpoint — no authentication required.
        /// Used by the MVC public lookup page via HttpClient.
        /// </summary>
        [HttpGet("certification")]
        public async Task<IActionResult> LookupCertification(
            [FromQuery] int traineeId,
            [FromQuery] string certificateNumber)
        {
            if (traineeId <= 0 || string.IsNullOrWhiteSpace(certificateNumber))
                return BadRequest(new { message = "Trainee ID and certificate number are required." });

            var cert = await _context.TraineeCertifications
                .Include(tc => tc.CertificationTrack)
                    .ThenInclude(ct => ct.CertificationRequiredCourses)
                        .ThenInclude(rc => rc.Course)
                .Include(tc => tc.Status)
                .Include(tc => tc.Trainee)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(tc =>
                    tc.TraineeId == traineeId &&
                    tc.CertificateNumber == certificateNumber.Trim());

            if (cert == null)
                return NotFound(new { message = "No certificate found matching the provided details." });

            var requiredCourseIds = cert.CertificationTrack.CertificationRequiredCourses
                .Select(rc => rc.CourseId)
                .ToHashSet();

            var completions = await _context.TraineeCourseCompletions
                .Include(c => c.Course)
                .Where(c => c.TraineeId == traineeId && requiredCourseIds.Contains(c.CourseId))
                .OrderBy(c => c.CompletionDate)
                .ToListAsync();

            var mandatoryMap = cert.CertificationTrack.CertificationRequiredCourses
                .ToDictionary(rc => rc.CourseId, rc => rc.IsMandatory);

            return Ok(new
            {
                traineeId = cert.TraineeId,
                traineeEmail = cert.Trainee.User.Email,
                certificateNumber = cert.CertificateNumber,
                certificationTrackName = cert.CertificationTrack.Name,
                status = cert.Status.Status,
                issuedDate = cert.CertificateIssuedDate,
                expiryDate = cert.ExpiryDate,
                completedCourses = completions.Select(c => new
                {
                    courseCode = c.Course.CourseCode,
                    title = c.Course.Title,
                    result = c.Result,
                    completionDate = c.CompletionDate,
                    isMandatory = mandatoryMap.TryGetValue(c.CourseId, out var m) && m
                }).ToList()
            });
        }
    }
}
