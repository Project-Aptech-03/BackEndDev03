using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.CustomerAddress
{
    public class UpdateCustomerAddressDto
    {
        [StringLength(100, ErrorMessage = "Tên ??a ch? không ???c v??t quá 100 ký t?.")]
        public string? AddressName { get; set; }

        [StringLength(500, ErrorMessage = "??a ch? ??y ?? không ???c v??t quá 500 ký t?.")]
        public string? FullAddress { get; set; }

        [StringLength(100, ErrorMessage = "Qu?n/Huy?n không ???c v??t quá 100 ký t?.")]
        public string? District { get; set; }

        [StringLength(100, ErrorMessage = "Thành ph? không ???c v??t quá 100 ký t?.")]
        public string? City { get; set; }

        [StringLength(10, ErrorMessage = "Mã b?u ?i?n không ???c v??t quá 10 ký t?.")]
        public string? PostalCode { get; set; }

        [Range(0, 999.99, ErrorMessage = "Kho?ng cách ph?i t? 0 ??n 999.99 km.")]
        public decimal? DistanceKm { get; set; }

        public bool? IsDefault { get; set; }

        public bool? IsActive { get; set; }
    }
}