using System.ComponentModel.DataAnnotations;

namespace MVC_Frontend.Models
{
    public class DropViewModel
    {
        public int EnrollmentId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public string CurrentStatus { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Reason for dropping (optional)")]
        public string? DropReason { get; set; }
    }
}
