using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.CustomerAddress
{
    public class UpdateCustomerAddressDto
    {
        [StringLength(100, ErrorMessage = "Address name cannot exceed 100 characters.")]
        public string? AddressName { get; set; }

        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string? FullName { get; set; }

        [StringLength(500, ErrorMessage = "Full address cannot exceed 500 characters.")]
        public string? FullAddress { get; set; }

        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        public string? PhoneNumber { get; set; }

        [Range(0, 999.99, ErrorMessage = "Distance must be between 0 and 999.99 km.")]
        public decimal? DistanceKm { get; set; }

        public bool? IsDefault { get; set; }

        public bool? IsActive { get; set; }
    }
}