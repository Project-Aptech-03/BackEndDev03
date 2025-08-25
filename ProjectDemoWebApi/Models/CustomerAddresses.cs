using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("CustomerAddresses")]
    public class CustomerAddresses
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(450)]
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        [Column("full_address")]
        public string FullAddress { get; set; } = string.Empty;

        [StringLength(100)]
        [Column("district")]
        public string? District { get; set; }

        [StringLength(100)]
        [Column("city")]
        public string? City { get; set; }

        [StringLength(10)]
        [Column("postal_code")]
        public string? PostalCode { get; set; }

        [Column("distance_km", TypeName = "decimal(5,2)")]
        public decimal? DistanceKm { get; set; }

        [Column("is_default")]
        public bool IsDefault { get; set; } = false;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        // Note: User relationship handled by foreign key constraint only
        
        public virtual ICollection<Orders> Orders { get; set; } = new List<Orders>();
    }
}