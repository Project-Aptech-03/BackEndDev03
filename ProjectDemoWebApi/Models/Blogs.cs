using ProjectDemoWebApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("Blogs")]
    public class Blogs
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Column("content", TypeName = "ntext")]
        public string Content { get; set; } = string.Empty;

        [StringLength(500)]
        [Column("summary")]
        public string? Summary { get; set; }

        [StringLength(500)]
        [Column("featured_image_url")]
        public string? FeaturedImageUrl { get; set; }

        [StringLength(100)]
        [Column("slug")]
        public string? Slug { get; set; }

        [Column("is_published")]
        public bool IsPublished { get; set; } = false;

        [Column("is_featured")]
        public bool IsFeatured { get; set; } = false;

        [Column("view_count")]
        public int ViewCount { get; set; } = 0;

        [Column("like_count")]
        public int LikeCount { get; set; } = 0;

        [Column("comment_count")]
        public int CommentCount { get; set; } = 0;

        [Column("published_date")]
        public DateTime? PublishedDate { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("updated_date")]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        [Column("author_id")]
        public string AuthorId { get; set; } = string.Empty;

        [Column("category_id")]
        public int? CategoryId { get; set; }

        // Navigation Properties
        [ForeignKey("AuthorId")]
        public virtual Users Author { get; set; } = null!;

        [ForeignKey("CategoryId")]
        public virtual Categories Category { get; set; } = null!;

        public virtual ICollection<BlogComments> Comments { get; set; } = new List<BlogComments>();
        public virtual ICollection<BlogLikes> Likes { get; set; } = new List<BlogLikes>();
    }
}