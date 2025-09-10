using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ProjectDemoWebApi.DTOs.Products
{
    public class CreateProductDto
    {
        public string ProductCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be greater than 0.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Manufacturer ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Manufacturer ID must be greater than 0.")]
        public int ManufacturerId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Publisher ID must be greater than 0.")]
        public int? PublisherId { get; set; }

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

        [Range(0.01, 9999.99, ErrorMessage = "Weight must be between 0.01 and 9999.99 kg.")]
        public decimal? Weight { get; set; }

        [Required(ErrorMessage = "Price cannot be empty.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
        public int StockQuantity { get; set; } = 0;
    }
}