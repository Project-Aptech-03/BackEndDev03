using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Manufacturer
{
    public class CreateManufacturerDto
    {
        [Required(ErrorMessage = "Manufacturer code cannot be empty.")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "Manufacturer code must be exactly 5 characters.")]
        [RegularExpression(@"^[A-Z]{3}\d{2}$", ErrorMessage = "Manufacturer code must follow format: 3 letters + 2 digits (e.g., ABC01).")]
        public string ManufacturerCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Manufacturer name cannot be empty.")]
        [StringLength(150, ErrorMessage = "Manufacturer name cannot exceed 150 characters.")]
        public string ManufacturerName { get; set; } = string.Empty;
    }
}