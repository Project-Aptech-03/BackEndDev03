using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("BlogComments")]
    public class BlogComments
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("content", TypeName = "ntext")]
        public string Content { get; set; } = string.Empty;

        [Column("is_approved")]
        public bool IsApproved { get; set; } = true;

        [Column("like_count")]
        public int LikeCount { get; set; } = 0;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("updated_date")]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        [Column("blog_id")]
        public int BlogId { get; set; }

        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Column("parent_comment_id")]
        public int? ParentCommentId { get; set; }

        // Navigation Properties
        [ForeignKey("BlogId")]
        public virtual Blogs Blog { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;

        [ForeignKey("ParentCommentId")]
        public virtual BlogComments? ParentComment { get; set; }

        public virtual ICollection<BlogComments> Replies { get; set; } = new List<BlogComments>();
        public virtual ICollection<CommentLikes> Likes { get; set; } = new List<CommentLikes>();
    }
}
