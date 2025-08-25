using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("OrderItems")]
    public class OrderItems
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [Column("quantity")]
        public int Quantity { get; set; } = 1;

        [Required]
        [Column("unit_price", TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        [Column("discount_percent", TypeName = "decimal(5,2)")]
        public decimal DiscountPercent { get; set; } = 0;

        [Column("discount_amount", TypeName = "decimal(8,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Required]
        [Column("total_price", TypeName = "decimal(12,2)")]
        public decimal TotalPrice { get; set; }

        [StringLength(255)]
        [Column("notes")]
        public string? Notes { get; set; }

        // Navigation Properties
        [ForeignKey("OrderId")]
        public virtual Orders? Order { get; set; }

        [ForeignKey("ProductId")]
        public virtual Products? Product { get; set; }
    }
}