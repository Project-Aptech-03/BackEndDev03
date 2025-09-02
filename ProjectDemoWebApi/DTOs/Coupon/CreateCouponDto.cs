using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Coupon
{
    public class CreateCouponDto
    {
        [Required(ErrorMessage = "M� coupon kh�ng ???c ?? tr?ng.")]
        [StringLength(50, ErrorMessage = "M� coupon kh�ng ???c v??t qu� 50 k� t?.")]
        public string CouponCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "T�n coupon kh�ng ???c ?? tr?ng.")]
        [StringLength(100, ErrorMessage = "T�n coupon kh�ng ???c v??t qu� 100 k� t?.")]
        public string CouponName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lo?i gi?m gi� kh�ng ???c ?? tr?ng.")]
        [RegularExpression("^(percentage|fixed)$", ErrorMessage = "Lo?i gi?m gi� ph?i l� 'percentage' ho?c 'fixed'.")]
        public string DiscountType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gi� tr? gi?m gi� kh�ng ???c ?? tr?ng.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Gi� tr? gi?m gi� ph?i l?n h?n 0.")]
        public decimal DiscountValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "S? ti?n ??n h�ng t?i thi?u ph?i l?n h?n ho?c b?ng 0.")]
        public decimal MinOrderAmount { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "S? ti?n gi?m gi� t?i ?a ph?i l?n h?n ho?c b?ng 0.")]
        public decimal MaxDiscountAmount { get; set; } = 0;

        [Required(ErrorMessage = "S? l??ng kh�ng ???c ?? tr?ng.")]
        [Range(-1, int.MaxValue, ErrorMessage = "S? l??ng ph?i l?n h?n ho?c b?ng -1 (v� h?n) ho?c l?n h?n 0.")]
        public int Quantity { get; set; } = 1;

        [Required(ErrorMessage = "Ng�y b?t ??u kh�ng ???c ?? tr?ng.")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ng�y k?t th�c kh�ng ???c ?? tr?ng.")]
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
                    "Ng�y k?t th�c ph?i sau ng�y b?t ??u.",
                    new[] { nameof(EndDate) });
            }

            if (StartDate < DateTime.Now.Date)
            {
                yield return new ValidationResult(
                    "Ng�y b?t ??u kh�ng ???c l� ng�y trong qu� kh?.",
                    new[] { nameof(StartDate) });
            }

            if (DiscountType == "percentage" && DiscountValue > 100)
            {
                yield return new ValidationResult(
                    "Gi� tr? gi?m gi� theo ph?n tr?m kh�ng ???c v??t qu� 100%.",
                    new[] { nameof(DiscountValue) });
            }
        }
    }
}