using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Products
{
    public class CreateProductsDto
    {
        [Required(ErrorMessage = "Product code cannot be empty.")]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "Product code must be exactly 7 characters.")]
        public string ProductCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category ID cannot be empty.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Manufacturer ID cannot be empty.")]
        public int ManufacturerId { get; set; }

        public int? PublisherId { get; set; }

        [Required(ErrorMessage = "Product name cannot be empty.")]
        [StringLength(255, ErrorMessage = "Product name cannot exceed 255 characters.")]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [StringLength(200, ErrorMessage = "Author cannot exceed 200 characters.")]
        public string? Author { get; set; }

        [StringLength(100, ErrorMessage = "Product type cannot exceed 100 characters.")]
        public string? ProductType { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Pages must be greater than 0.")]
        public int? Pages { get; set; }

        [StringLength(50, ErrorMessage = "Dimensions cannot exceed 50 characters.")]
        public string? Dimensions { get; set; }

        [Range(0.01, 999.99, ErrorMessage = "Weight must be between 0.01 and 999.99 kg.")]
        public decimal? Weight { get; set; }

        [Required(ErrorMessage = "Price cannot be empty.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
        public int StockQuantity { get; set; } = 0;

        public List<IFormFile>? Photos { get; set; }
    }
}