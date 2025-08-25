using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.CustomerAddress
{
    public class UpdateCustomerAddressDto
    {
        [StringLength(500, ErrorMessage = "Full address cannot exceed 500 characters.")]
        public string? FullAddress { get; set; }

        [StringLength(100, ErrorMessage = "District cannot exceed 100 characters.")]
        public string? District { get; set; }

        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
        public string? City { get; set; }

        [StringLength(10, ErrorMessage = "Postal code cannot exceed 10 characters.")]
        public string? PostalCode { get; set; }

        [Range(0, 999.99, ErrorMessage = "Distance must be between 0 and 999.99 km.")]
        public decimal? DistanceKm { get; set; }

        public bool? IsDefault { get; set; }

        public bool? IsActive { get; set; }
    }
}