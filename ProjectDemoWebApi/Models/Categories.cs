using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("Categories")]
    public class Categories
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(2)]
        [Column("category_code")]
        public string CategoryCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("category_name")]
        public string CategoryName { get; set; } = string.Empty;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ICollection<Products> Products { get; set; } = new List<Products>();
    }
}
