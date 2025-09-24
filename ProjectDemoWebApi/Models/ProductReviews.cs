using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("ProductReviews")]
    public class ProductReviews
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
        [StringLength(450)]
        [Column("customer_id")]
        public string CustomerId { get; set; } = string.Empty;

        [Required]
        [Range(1, 5)]
        [Column("rating")]
        public int Rating { get; set; }

        [Column("comment", TypeName = "nvarchar(max)")]
        public string? Comment { get; set; }

        [Column("review_date")]
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        [Column("is_approved")]
        public bool IsApproved { get; set; } = true;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("updated_date")]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("OrderId")]
        public virtual Orders? Order { get; set; }

        [ForeignKey("ProductId")]
        public virtual Products? Product { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Users? Customer { get; set; }

        public virtual ICollection<ReviewImages> ReviewImages { get; set; } = new List<ReviewImages>();
        public virtual ICollection<ReviewReplies> ReviewReplies { get; set; } = new List<ReviewReplies>();
    }
}