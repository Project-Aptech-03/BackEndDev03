using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("CustomerQueries")]
    public class CustomerQueries
    {
        [Key]
        public int Id { get; set; }

        [StringLength(450)]
        [Column("customer_id")]
        public string? CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("customer_name")]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column("customer_email")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column("subject")]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [Column("message", TypeName = "text")]
        public string Message { get; set; } = string.Empty;

        [StringLength(50)]
        [Column("status")]
        public string Status { get; set; } = "Open";

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        // Note: Customer relationship handled by foreign key constraint only
        
        public virtual ICollection<AdminReplies> AdminReplies { get; set; } = new List<AdminReplies>();
        [ForeignKey("CustomerId")]
        public virtual Users? Customer { get; set; }

    }
}