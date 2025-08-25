using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("Manufacturers")]
    public class Manufacturers
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(5)]
        [Column("manufacturer_code")]
        public string ManufacturerCode { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [Column("manufacturer_name")]
        public string ManufacturerName { get; set; } = string.Empty;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ICollection<Products> Products { get; set; } = new List<Products>();
    }
}