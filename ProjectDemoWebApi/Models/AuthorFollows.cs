
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("AuthorFollows")]
    public class AuthorFollows
    {
        [Key]
        public int Id { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        [Column("follower_id")]
        public string FollowerId { get; set; } = string.Empty;

        [Column("author_id")]
        public string AuthorId { get; set; } = string.Empty;

        // Navigation Properties
        [ForeignKey("FollowerId")]
        public virtual Users Follower { get; set; } = null!;

        [ForeignKey("AuthorId")]
        public virtual Users Author { get; set; } = null!;
    }
}