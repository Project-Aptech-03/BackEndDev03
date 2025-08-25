using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Manufacturer
{
    public class UpdateManufacturerDto
    {
        [StringLength(150, ErrorMessage = "Manufacturer name cannot exceed 150 characters.")]
        public string? ManufacturerName { get; set; }

        public bool? IsActive { get; set; }
    }
}