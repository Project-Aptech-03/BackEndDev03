using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("ShoppingCart")]
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(450)]
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [Column("quantity")]
        public int Quantity { get; set; } = 1;

        [Required]
        [Column("unit_price", TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        [Column("added_date")]
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        [Column("updated_date")]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties        
        [ForeignKey("ProductId")]
        public virtual Products? Product { get; set; }
        [ForeignKey("UserId")]
        public virtual Users? User { get; set; }

    }
}