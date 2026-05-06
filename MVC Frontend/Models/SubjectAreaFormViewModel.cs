using System.ComponentModel.DataAnnotations;

namespace MVC_Frontend.Models
{
    public class SubjectAreaFormViewModel
    {
        public int SubjectAreaId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Name must be 100 characters or fewer.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
