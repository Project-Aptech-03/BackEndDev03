using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Coupon
{
    public class ApplyCouponDto
    {
        [Required(ErrorMessage = "The coupon code cannot be empty.")]
        [StringLength(50, ErrorMessage = "The coupon code cannot exceed 50 characters.")]
        public string CouponCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "The order amount cannot be empty.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "The order amount must be greater than 0.")]
        public decimal OrderAmount { get; set; }
    }

    public class CouponDiscountResultDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public CouponResponseDto? CouponInfo { get; set; }
    }

    public class ValidateCouponDto
    {
        [Required(ErrorMessage = "The coupon code cannot be empty.")]
        public string CouponCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "The order amount cannot be empty.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "The order amount must be greater than 0.")]
        public decimal OrderAmount { get; set; }
    }
}