using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Coupon
{
    public class CreateCouponDto
    {
        [Required(ErrorMessage = "Coupon code cannot be empty.")]
        [StringLength(50, ErrorMessage = "Coupon code cannot exceed 50 characters.")]
        public string CouponCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Coupon name cannot be empty.")]
        [StringLength(100, ErrorMessage = "Coupon name cannot exceed 100 characters.")]
        public string CouponName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Discount type cannot be empty.")]
        [RegularExpression("^(percentage|fixed)$", ErrorMessage = "Discount type must be 'percentage' or 'fixed'.")]
        public string DiscountType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Discount value cannot be empty.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Discount value must be greater than 0.")]
        public decimal DiscountValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Minimum order amount must be greater than or equal to 0.")]
        public decimal MinOrderAmount { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "Maximum discount amount must be greater than or equal to 0.")]
        public decimal MaxDiscountAmount { get; set; } = 0;

        [Required(ErrorMessage = "Quantity cannot be empty.")]
        [Range(-1, int.MaxValue, ErrorMessage = "Quantity must be greater than or equal to -1 (infinite) or greater than 0.")]
        public int Quantity { get; set; } = 1;

        [Required(ErrorMessage = "Start date cannot be empty.")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date cannot be empty.")]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        public bool IsAutoApply { get; set; } = false;

        public bool IsActive { get; set; } = true;

        // Custom validation for date range
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate <= StartDate)
            {
                yield return new ValidationResult(
                    "End date must be after start date.",
                    new[] { nameof(EndDate) });
            }

            if (StartDate < DateTime.Now.Date)
            {
                yield return new ValidationResult(
                    "Start date cannot be in the past.",
                    new[] { nameof(StartDate) });
            }

            if (DiscountType == "percentage" && DiscountValue > 100)
            {
                yield return new ValidationResult(
                    "Percentage discount value cannot exceed 100%.",
                    new[] { nameof(DiscountValue) });
            }
        }
    }
}
