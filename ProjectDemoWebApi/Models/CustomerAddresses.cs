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
        [StringLength(100)]
        [Column("address_name")]
        public string AddressName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("full_name")]
        public string FullName { get; set; } = string.Empty;       

        [Required]
        [StringLength(500)]
        [Column("full_address")]
        public string FullAddress { get; set; } = string.Empty;

        [StringLength(15)]
        [Column("phone_number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Column("distance_km", TypeName = "decimal(5,2)")]
        public decimal? DistanceKm { get; set; }

        [Column("is_default")]
        public bool IsDefault { get; set; } = false;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
                
        [ForeignKey("UserId")]
        public virtual Users? User { get; set; }
    }
}