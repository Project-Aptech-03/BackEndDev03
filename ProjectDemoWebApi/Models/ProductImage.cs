using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjectDemoWebApi.Models
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsMain { get; set; } = false; 

        public int OrderIndex { get; set; } = 0; 

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        [JsonIgnore]
        public Product Product { get; set; } = null!;
    }
}
