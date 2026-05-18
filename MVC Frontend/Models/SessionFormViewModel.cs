using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC_Frontend.Models
{
    public class SessionFormViewModel
    {
        public int SessionId { get; set; }

        [Required(ErrorMessage = "Please select a course.")]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Please select an instructor.")]
        [Display(Name = "Instructor")]
        public int InstructorId { get; set; }

        [Required(ErrorMessage = "Please select a classroom.")]
        [Display(Name = "Classroom")]
        public int ClassroomId { get; set; }

        [Required(ErrorMessage = "Please enter the session date.")]
        [Display(Name = "Session Date")]
        [DataType(DataType.Date)]
        public DateTime SessionDate { get; set; } = DateTime.Today.AddDays(1);

        [Required(ErrorMessage = "Please enter the start time.")]
        [Display(Name = "Start Time")]
        public string StartTime { get; set; } = "09:00";

        [Required(ErrorMessage = "Please enter the end time.")]
        [Display(Name = "End Time")]
        public string EndTime { get; set; } = "17:00";

        [Required(ErrorMessage = "Please enter the maximum capacity.")]
        [Display(Name = "Max Capacity")]
        [Range(1, 1000, ErrorMessage = "Capacity must be between 1 and 1000.")]
        public int MaxCapacity { get; set; }

        [Required]
        [Display(Name = "Status")]
        public int StatusId { get; set; } = 1;

        public IEnumerable<SelectListItem> Courses { get; set; } = [];
        public IEnumerable<SelectListItem> Instructors { get; set; } = [];
        public IEnumerable<SelectListItem> Classrooms { get; set; } = [];
        public IEnumerable<SelectListItem> Statuses { get; set; } = [];
    }
}
