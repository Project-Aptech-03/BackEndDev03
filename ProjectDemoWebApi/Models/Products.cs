using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    [Table("Products")]
    public class Products
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(7)]
        [Column("product_code")]
        public string ProductCode { get; set; } = string.Empty;

        [Required]
        [Column("category_id")]
        public int CategoryId { get; set; }

        [Required]
        [Column("manufacturer_id")]
        public int ManufacturerId { get; set; }

        [Column("publisher_id")]
        public int? PublisherId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("product_name")]
        public string ProductName { get; set; } = string.Empty;

        [Column("description", TypeName = "nvarchar(max)")]
        public string? Description { get; set; }

        [StringLength(200)]
        [Column("author")]
        public string? Author { get; set; }

        [StringLength(100)]
        [Column("product_type")]
        public string? ProductType { get; set; }

        [Column("pages")]
        public int? Pages { get; set; }

        [Column("dimension_length")]
        public int? DimensionLength { get; set; }

        [Column("dimension_width")]
        public int? DimensionWidth { get; set; }

        [Column("dimension_height")]
        public int? DimensionHeight { get; set; }

        [Column("weight", TypeName = "decimal(5,2)")]
        public decimal? Weight { get; set; }

        [Required]
        [Column("price", TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Column("stock_quantity")]
        public int StockQuantity { get; set; } = 0;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("CategoryId")]
        public virtual Categories? Category { get; set; }

        [ForeignKey("ManufacturerId")]
        public virtual Manufacturers? Manufacturer { get; set; }

        [ForeignKey("PublisherId")]
        public virtual Publishers? Publisher { get; set; }

        public virtual ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
        public virtual ICollection<ProductPhotos> ProductPhotos { get; set; } = new List<ProductPhotos>();
        public virtual ICollection<ShoppingCart> ShoppingCartItems { get; set; } = new List<ShoppingCart>();
        public virtual ICollection<StockMovements> StockMovements { get; set; } = new List<StockMovements>();


        //====Sinhnd-Cập nhật Products.cs để thêm navigation property cho reviews=============
        public virtual ICollection<ProductReviews> ProductReviews { get; set; } = new List<ProductReviews>();
        //=====================================================================
    }
}
