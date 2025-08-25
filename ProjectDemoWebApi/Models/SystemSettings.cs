using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("SystemSettings")]
    public class SystemSettings
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Column("setting_key")]
        public string SettingKey { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        [Column("setting_value")]
        public string SettingValue { get; set; } = string.Empty;

        [StringLength(255)]
        [Column("description")]
        public string? Description { get; set; }

        [Column("updated_date")]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}