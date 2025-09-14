using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("Publishers")]
    public class Publishers
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        [Column("publisher_name")]
        public string PublisherName { get; set; } = string.Empty;

        [Column("publisher_address", TypeName = "nvarchar(max)")]
        public string? PublisherAddress { get; set; }


        [StringLength(255)]
        [Column("contact_info")]
        public string? ContactInfo { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ICollection<Products> Products { get; set; } = new List<Products>();
    }
}