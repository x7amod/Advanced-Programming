using System.ComponentModel.DataAnnotations;

namespace MVC_Frontend.Models;

public class ClassroomFormViewModel
{
    public int ClassroomId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = "";

    [Required]
    [StringLength(200)]
    public string Location { get; set; } = "";

    [Required]
    [StringLength(100)]
    public string Building { get; set; } = "";

    [Required]
    [StringLength(20)]
    public string Floor { get; set; } = "";

    [Required]
    [Range(1, 1000, ErrorMessage = "Capacity must be between 1 and 1000.")]
    public int Capacity { get; set; }

    public bool IsActive { get; set; } = true;
}

public class ClassroomEquipmentFormViewModel
{
    [Required]
    public int ClassroomId { get; set; }

    public int? EquipmentId { get; set; }

    [Required]
    [StringLength(50)]
    public string EquipmentType { get; set; } = "";

    [Required]
    [Range(1, 9999)]
    public int Quantity { get; set; } = 1;

    [StringLength(255)]
    public string? Description { get; set; }
}
