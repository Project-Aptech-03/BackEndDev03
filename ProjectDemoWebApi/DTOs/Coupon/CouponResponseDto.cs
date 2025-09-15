namespace ProjectDemoWebApi.DTOs.Coupon
{
    public class CouponResponseDto
    {
        public int Id { get; set; }
        public string CouponCode { get; set; } = string.Empty;
        public string CouponName { get; set; } = string.Empty;
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public decimal MinOrderAmount { get; set; }
        public decimal MaxDiscountAmount { get; set; }
        public int Quantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsAutoApply { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        // Computed properties
        public bool IsValid => IsActive &&
                               StartDate <= DateTime.UtcNow &&
                               EndDate >= DateTime.UtcNow &&
                               (Quantity > 0 || Quantity == -1);

        public bool IsExpired => EndDate < DateTime.UtcNow;

        public string Status => IsExpired ? "Expired" :
                               !IsActive ? "Inactive" :
                               StartDate > DateTime.UtcNow ? "Not Started" :
                               IsValid ? "Active" : "Out of Stock";

        public string DiscountDisplay => DiscountType == "percentage"
            ? $"{DiscountValue}%"
            : $"{DiscountValue:C}";
    }
}