using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("BlogLikes")]
    public class BlogLikes
    {
        [Key]
        public int Id { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        [Column("blog_id")]
        public int BlogId { get; set; }

        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        // Navigation Properties
        [ForeignKey("BlogId")]
        public virtual Blogs Blog { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;
    }
}
