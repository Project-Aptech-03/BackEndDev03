using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("AdminReplies")]
    public class AdminReplies
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("query_id")]
        public int QueryId { get; set; }

        [Required]
        [StringLength(450)]
        [Column("admin_id")]
        public string AdminId { get; set; } = string.Empty;

        [Required]
        [Column("reply_message", TypeName = "text")]
        public string ReplyMessage { get; set; } = string.Empty;

        [Column("reply_date")]
        public DateTime ReplyDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("QueryId")]
        public virtual CustomerQueries? Query { get; set; }

        // Note: Admin relationship handled by foreign key constraint only
    }
}