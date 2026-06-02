using System.ComponentModel.DataAnnotations;

namespace MVC_Frontend.Models
{
    public class CertificationLookupViewModel
    {
        [Required(ErrorMessage = "Trainee ID is required.")]
        [Display(Name = "Trainee ID")]
        public int? TraineeId { get; set; }

        [Required(ErrorMessage = "Certificate number is required.")]
        [Display(Name = "Certificate Number")]
        [StringLength(50)]
        public string CertificateNumber { get; set; } = string.Empty;

        public CertificationLookupResult? Result { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class CertificationLookupResult
    {
        public int TraineeId { get; set; }
        public string TraineeEmail { get; set; } = string.Empty;
        public string CertificateNumber { get; set; } = string.Empty;
        public string CertificationTrackName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? IssuedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public List<CompletedCourseInfo> CompletedCourses { get; set; } = new();
    }

    public class CompletedCourseInfo
    {
        public string CourseCode { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public DateTime CompletionDate { get; set; }
        public bool IsMandatory { get; set; }
    }
}
