using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC_Frontend.Models
{
    public class CertificationTrackFormViewModel
    {
        public int? CertificationTrackId { get; set; }

        [Required, StringLength(150)]
        [Display(Name = "Track Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Range(1, 120, ErrorMessage = "Validity period must be between 1 and 120 months.")]
        [Display(Name = "Validity Period (months)")]
        public int? ValidityPeriod { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }

    public class AddCourseToTrackViewModel
    {
        public int CertificationTrackId { get; set; }
        public string TrackName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a course.")]
        [Display(Name = "Course")]
        public int? CourseId { get; set; }

        [Range(1, 99, ErrorMessage = "Sequence order must be between 1 and 99.")]
        [Display(Name = "Sequence Order")]
        public int? SequenceOrder { get; set; }

        [Display(Name = "Mandatory")]
        public bool IsMandatory { get; set; } = true;

        public IEnumerable<SelectListItem> AvailableCourses { get; set; } = Enumerable.Empty<SelectListItem>();
    }

    public class CourseProgressItem
    {
        public int CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
        public int? SequenceOrder { get; set; }
        public bool IsCompleted { get; set; }
        public string? Result { get; set; }
        public DateTime? CompletionDate { get; set; }
    }

    public class CertificationProgressViewModel
    {
        public Web_API.Models.CertificationTrack Track { get; set; } = null!;
        public Web_API.Models.TraineeCertification? TraineeCert { get; set; }
        public List<CourseProgressItem> Courses { get; set; } = new();
        public int TotalMandatory { get; set; }
        public int CompletedMandatory { get; set; }
        public bool IsEligible => TotalMandatory > 0 && CompletedMandatory >= TotalMandatory;
        public int ProgressPercent => TotalMandatory > 0
            ? (int)Math.Round(CompletedMandatory * 100.0 / TotalMandatory)
            : 0;
        public string? TraineeName { get; set; }
        public int? TraineeId { get; set; }
    }

    public class TrackEnrollmentSummaryViewModel
    {
        public int TraineeId { get; set; }
        public string TraineeName { get; set; } = string.Empty;
        public int CompletedMandatory { get; set; }
        public int TotalMandatory { get; set; }
        public int ProgressPercent => TotalMandatory > 0
            ? (int)Math.Round(CompletedMandatory * 100.0 / TotalMandatory)
            : 0;
        public bool IsEligible => TotalMandatory > 0 && CompletedMandatory >= TotalMandatory;
        public Web_API.Models.TraineeCertification? Certification { get; set; }
    }

    public class CertificateViewModel
    {
        public string TraineeName { get; set; } = "";
        public string TrackName { get; set; } = "";
        public string? TrackDescription { get; set; }
        public string CertificateNumber { get; set; } = "";
        public DateTime IssuedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public List<string> CompletedCourses { get; set; } = new();
    }
}
