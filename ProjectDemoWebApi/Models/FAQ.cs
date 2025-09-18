using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("FAQ")]
    public class FAQ
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("question", TypeName = "text")]
        public string Question { get; set; } = string.Empty;

        [Required]
        [Column("answer", TypeName = "text")]
        public string Answer { get; set; } = string.Empty;

        [Column("sort_order")]
        public int SortOrder { get; set; } = 0;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;
        public DateTime? UpdatedAt { get; set; }

        public int DisplayOrder { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
    
}