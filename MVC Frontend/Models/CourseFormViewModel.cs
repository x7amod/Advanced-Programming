using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC_Frontend.Models
{
    public class CourseFormViewModel
    {
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Please select a subject area.")]
        [Display(Name = "Subject Area")]
        public int SubjectAreaId { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Prerequisite Course")]
        public int? PrerequisiteCourseId { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Course code must be 30 characters or fewer.")]
        [Display(Name = "Course Code")]
        public string CourseCode { get; set; } = string.Empty;

        [Required]
        [StringLength(150, ErrorMessage = "Title must be 150 characters or fewer.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(0.5, 999.99, ErrorMessage = "Duration must be between 0.5 and 999.99 hours.")]
        [Display(Name = "Duration (Hours)")]
        public decimal DurationHours { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "Capacity must be between 1 and 1000.")]
        [Display(Name = "Max Capacity")]
        public int MaxCapacity { get; set; }

        [Required]
        [Range(0, 99999.99, ErrorMessage = "Fee must be between 0 and 99,999.99.")]
        [Display(Name = "Enrollment Fee (BHD)")]
        public decimal EnrollmentFee { get; set; }

        [StringLength(500)]
        [Display(Name = "Equipment Requirements")]
        public string? EquipmentRequirements { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        // Populated by controller before rendering the form
        public IEnumerable<SelectListItem> SubjectAreas { get; set; } = [];
        public IEnumerable<SelectListItem> Categories { get; set; } = [];
        public IEnumerable<SelectListItem> PrerequisiteCourses { get; set; } = [];
    }
}
