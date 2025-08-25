using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("Payments")]
    public class Payments
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("payment_method")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required]
        [Column("amount", TypeName = "decimal(12,2)")]
        public decimal Amount { get; set; }

        [StringLength(100)]
        [Column("transaction_id")]
        public string? TransactionId { get; set; }

        [StringLength(50)]
        [Column("payment_status")]
        public string PaymentStatus { get; set; } = "Pending";

        [Column("payment_date")]
        public DateTime? PaymentDate { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("OrderId")]
        public virtual Orders? Order { get; set; }
    }
}