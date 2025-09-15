using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Manufacturer
{
    public class CreateManufacturerDto
    {
        [Required(ErrorMessage = "Manufacturer code cannot be empty.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Manufacturer code must be exactly 3 characters.")]
        [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Manufacturer code must follow format: 3 letters")]
        public string ManufacturerCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Manufacturer name cannot be empty.")]
        [StringLength(150, ErrorMessage = "Manufacturer name cannot exceed 150 characters.")]
        public string ManufacturerName { get; set; } = string.Empty;
    }
}

