using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("Coupons")]
    public class Coupons
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Column("coupon_code")]
        public string CouponCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("coupon_name")]
        public string CouponName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("discount_type")]
        public string DiscountType { get; set; } = string.Empty;

        [Required]
        [Column("discount_value", TypeName = "decimal(8,2)")]
        public decimal DiscountValue { get; set; }

        [Column("min_order_amount", TypeName = "decimal(10,2)")]
        public decimal MinOrderAmount { get; set; } = 0;
        [Column("max_discount_amount", TypeName = "decimal(8,2)")]
        public decimal MaxDiscountAmount { get; set; } = 0;
        [Required]
        [Column("quantity")]
        public int Quantity { get; set; } = 1;
        [Required]
        [Column("start_date")]
        public DateTime StartDate { get; set; }
        [Required]
        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("is_auto_apply")]
        public bool IsAutoApply { get; set; } = false;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}