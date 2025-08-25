using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("ProductReturns")]
    public class ProductReturns
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(8)]
        [Column("return_number")]
        public string ReturnNumber { get; set; } = string.Empty;

        [Required]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Required]
        [StringLength(450)]
        [Column("customer_id")]
        public string CustomerId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("reason")]
        public string Reason { get; set; } = string.Empty;

        [StringLength(50)]
        [Column("status")]
        public string Status { get; set; } = "Pending";

        [Column("refund_amount", TypeName = "decimal(10,2)")]
        public decimal RefundAmount { get; set; } = 0;

        [Column("return_date")]
        public DateTime ReturnDate { get; set; } = DateTime.UtcNow;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("OrderId")]
        public virtual Orders? Order { get; set; }
    }
}