using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("ReviewImages")]
    public class ReviewImages
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("review_id")]
        public int ReviewId { get; set; }

        [Required]
        [StringLength(500)]
        [Column("image_url")]
        public string ImageUrl { get; set; } = string.Empty;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("ReviewId")]
        public virtual ProductReviews? Review { get; set; }
    }
}