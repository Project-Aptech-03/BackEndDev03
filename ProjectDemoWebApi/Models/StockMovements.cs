using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("StockMovements")]
    public class StockMovements
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [Column("quantity")]
        public int Quantity { get; set; }

        [Required]
        [Column("previous_stock")]
        public int PreviousStock { get; set; }

        [Required]
        [Column("new_stock")]
        public int NewStock { get; set; }

        [Required]
        [StringLength(50)]
        [Column("reference_type")]
        public string ReferenceType { get; set; } = string.Empty;

        [Column("reference_id")]
        public int? ReferenceId { get; set; }

        [Column("unit_cost", TypeName = "decimal(10,2)")]
        public decimal UnitCost { get; set; } = 0;

        [StringLength(255)]
        [Column("reason")]
        public string? Reason { get; set; }

        [Required]
        [StringLength(450)]
        [Column("created_by")]
        public string CreatedBy { get; set; } = string.Empty;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("ProductId")]
        public virtual Products? Product { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual Users? Creator { get; set; }

    }
}