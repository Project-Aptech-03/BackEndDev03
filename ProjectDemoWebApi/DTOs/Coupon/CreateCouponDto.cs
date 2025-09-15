using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Coupon
{
    public class CreateCouponDto
    {
        [Required(ErrorMessage = "Mã coupon không ???c ?? tr?ng.")]
        [StringLength(50, ErrorMessage = "Mã coupon không ???c v??t quá 50 ký t?.")]
        public string CouponCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên coupon không ???c ?? tr?ng.")]
        [StringLength(100, ErrorMessage = "Tên coupon không ???c v??t quá 100 ký t?.")]
        public string CouponName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lo?i gi?m giá không ???c ?? tr?ng.")]
        [RegularExpression("^(percentage|fixed)$", ErrorMessage = "Lo?i gi?m giá ph?i là 'percentage' ho?c 'fixed'.")]
        public string DiscountType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá tr? gi?m giá không ???c ?? tr?ng.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá tr? gi?m giá ph?i l?n h?n 0.")]
        public decimal DiscountValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "S? ti?n ??n hàng t?i thi?u ph?i l?n h?n ho?c b?ng 0.")]
        public decimal MinOrderAmount { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "S? ti?n gi?m giá t?i ?a ph?i l?n h?n ho?c b?ng 0.")]
        public decimal MaxDiscountAmount { get; set; } = 0;

        [Required(ErrorMessage = "S? l??ng không ???c ?? tr?ng.")]
        [Range(-1, int.MaxValue, ErrorMessage = "S? l??ng ph?i l?n h?n ho?c b?ng -1 (vô h?n) ho?c l?n h?n 0.")]
        public int Quantity { get; set; } = 1;

        [Required(ErrorMessage = "Ngày b?t ??u không ???c ?? tr?ng.")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày k?t thúc không ???c ?? tr?ng.")]
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
                    "Ngày k?t thúc ph?i sau ngày b?t ??u.",
                    new[] { nameof(EndDate) });
            }

            if (StartDate < DateTime.Now.Date)
            {
                yield return new ValidationResult(
                    "Ngày b?t ??u không ???c là ngày trong quá kh?.",
                    new[] { nameof(StartDate) });
            }

            if (DiscountType == "percentage" && DiscountValue > 100)
            {
                yield return new ValidationResult(
                    "Giá tr? gi?m giá theo ph?n tr?m không ???c v??t quá 100%.",
                    new[] { nameof(DiscountValue) });
            }
        }
    }
}