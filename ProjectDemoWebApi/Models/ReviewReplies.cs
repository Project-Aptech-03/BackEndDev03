using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("ReviewReplies")]
    public class ReviewReplies
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("review_id")]
        public int ReviewId { get; set; }

        [Column("parent_reply_id")]
        public int? ParentReplyId { get; set; }

        [Required]
        [StringLength(450)]
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column("is_admin_reply")]
        public bool IsAdminReply { get; set; }

        [Required]
        [Column("comment", TypeName = "nvarchar(max)")]
        public string Comment { get; set; } = string.Empty;

        [Column("reply_date")]
        public DateTime ReplyDate { get; set; } = DateTime.UtcNow;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("ReviewId")]
        public virtual ProductReviews? Review { get; set; }

        [ForeignKey("ParentReplyId")]
        public virtual ReviewReplies? ParentReply { get; set; }

        [ForeignKey("UserId")]
        public virtual Users? User { get; set; }

        public virtual ICollection<ReviewReplies> ChildReplies { get; set; } = new List<ReviewReplies>();
    }
}