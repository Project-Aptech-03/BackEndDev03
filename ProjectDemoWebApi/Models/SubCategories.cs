using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("SubCategories")]
    public class SubCategories
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(15)]
        [Column("subcategory_code")]
        public string SubCategoryCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("subcategory_name")]
        public string SubCategoryName { get; set; } = string.Empty;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("Category")]
        public int? CategoryId { get; set; }

        public virtual Categories? Category { get; set; }
    }
}
