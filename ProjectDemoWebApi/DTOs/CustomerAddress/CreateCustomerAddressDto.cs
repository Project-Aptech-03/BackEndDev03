using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.CustomerAddress
{
    public class CreateCustomerAddressDto
    {
        [Required(ErrorMessage = "Address name is required.")]
        [StringLength(100, ErrorMessage = "Address name cannot exceed 100 characters.")]
        public string AddressName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full address is required.")]
        [StringLength(500, ErrorMessage = "Full address cannot exceed 500 characters.")]
        public string FullAddress { get; set; } = string.Empty;

        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Range(0, 999.99, ErrorMessage = "Distance must be between 0 and 999.99 km.")]
        public decimal? DistanceKm { get; set; }

        public bool IsDefault { get; set; } = false;
    }
}