using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("Orders")]
    public class Orders
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(8)]
        [Column("order_number")]
        public string OrderNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(450)]
        [Column("customer_id")]
        public string CustomerId { get; set; } = string.Empty;

        [Required]
        [Column("delivery_address_id")]
        public int DeliveryAddressId { get; set; }

        [Column("order_date")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("subtotal", TypeName = "decimal(12,2)")]
        public decimal Subtotal { get; set; }

        [Column("coupon_discount_amount", TypeName = "decimal(8,2)")]
        public decimal CouponDiscountAmount { get; set; } = 0;

        [Column("delivery_charges", TypeName = "decimal(8,2)")]
        public decimal DeliveryCharges { get; set; } = 0;

        [Required]
        [Column("total_amount", TypeName = "decimal(12,2)")]
        public decimal TotalAmount { get; set; }

        [StringLength(50)]
        [Column("order_status")]
        public string OrderStatus { get; set; } = "Pending";

        [Required]
        [StringLength(50)]
        [Column("payment_type")]
        public string PaymentType { get; set; } = string.Empty;

        [StringLength(50)]
        [Column("payment_status")]
        public string PaymentStatus { get; set; } = "Pending";

        [Column("applied_coupons", TypeName = "text")]
        public string? AppliedCoupons { get; set; }

        [Column("delivery_notes", TypeName = "text")]
        public string? DeliveryNotes { get; set; }

        [Column("cancellation_reason", TypeName = "text")]
        public string? CancellationReason { get; set; }

        [Column("cancelled_date")]
        public DateTime? CancelledDate { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("updated_date")]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        // Note: Customer relationship handled by foreign key constraint only
        
        [ForeignKey("DeliveryAddressId")]
        public virtual CustomerAddresses? DeliveryAddress { get; set; }

        public virtual ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
        public virtual ICollection<Payments> Payments { get; set; } = new List<Payments>();
        public virtual ICollection<ProductReturns> ProductReturns { get; set; } = new List<ProductReturns>();

        [ForeignKey("CustomerId")]
        public virtual Users? Customer { get; set; }


        //=========SinhND-Cập nhật Orders.cs để thêm navigation property cho reviews=========
        public virtual ICollection<ProductReviews> ProductReviews { get; set; } = new List<ProductReviews>();

        //=====================
    }
}
