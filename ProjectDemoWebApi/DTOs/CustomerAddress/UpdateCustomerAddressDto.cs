using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.CustomerAddress
{
    public class UpdateCustomerAddressDto
    {
        [StringLength(100, ErrorMessage = "T�n ??a ch? kh�ng ???c v??t qu� 100 k� t?.")]
        public string? AddressName { get; set; }

        [StringLength(500, ErrorMessage = "??a ch? ??y ?? kh�ng ???c v??t qu� 500 k� t?.")]
        public string? FullAddress { get; set; }

        [StringLength(100, ErrorMessage = "Qu?n/Huy?n kh�ng ???c v??t qu� 100 k� t?.")]
        public string? District { get; set; }

        [StringLength(100, ErrorMessage = "Th�nh ph? kh�ng ???c v??t qu� 100 k� t?.")]
        public string? City { get; set; }

        [StringLength(10, ErrorMessage = "M� b?u ?i?n kh�ng ???c v??t qu� 10 k� t?.")]
        public string? PostalCode { get; set; }

        [Range(0, 999.99, ErrorMessage = "Kho?ng c�ch ph?i t? 0 ??n 999.99 km.")]
        public decimal? DistanceKm { get; set; }

        public bool? IsDefault { get; set; }

        public bool? IsActive { get; set; }
    }
}