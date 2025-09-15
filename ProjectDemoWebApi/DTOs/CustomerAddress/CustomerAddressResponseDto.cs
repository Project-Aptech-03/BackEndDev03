using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.CustomerAddress
{
    public class CustomerAddressResponseDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string AddressName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public decimal? DistanceKm { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        // Calculated properties
        public string DisplayAddress => $"{AddressName}: {FullAddress}".Trim(' ', ',', ':');
        public string DisplayDistance => DistanceKm.HasValue && DistanceKm > 0 ? $"{DistanceKm:F2} km from store" : "Distance not determined";
        public string DisplayContactInfo => $"{FullName} - {PhoneNumber}".Trim(' ', '-');

        // Shipping fee calculation
        public decimal ShippingFee => CalculateShippingFee();
        public string DisplayShippingFee => ShippingFee == 0 ? "Free shipping" : $"{ShippingFee:N0} VND";

        private decimal CalculateShippingFee()
        {
            if (!DistanceKm.HasValue || DistanceKm <= 0)
                return 0;

            // Free shipping for distance under 3km
            if (DistanceKm < 3)
                return 0;

            // For distance >= 3km: 15,000 VND base + 5,000 VND per km
            decimal baseFee = 15000;
            decimal additionalFee = (decimal)(DistanceKm-3) * 5000;

            return baseFee + additionalFee;
        }
    }
}
