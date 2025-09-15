using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("CommentLikes")]
    public class CommentLikes
    {
        [Key]
        public int Id { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        [Column("comment_id")]
        public int CommentId { get; set; }

        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        // Navigation Properties
        [ForeignKey("CommentId")]
        public virtual BlogComments Comment { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;
    }
}