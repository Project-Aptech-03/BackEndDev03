using ProjectDemoWebApi.DTOs.Category;
using ProjectDemoWebApi.DTOs.Manufacturer;
using ProjectDemoWebApi.DTOs.Publisher;
using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Products
{
    public class ProductsResponseDto
    {
        public int Id { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Author { get; set; }
        public string? ProductType { get; set; }
        public int? Pages { get; set; }
        public int? DimensionLength { get; set; }

        public int? DimensionWidth { get; set; }

        public int? DimensionHeight { get; set; }

        public decimal? Weight { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        // Related entities
        public CategoryResponseDto? Category { get; set; }
        public ManufacturerResponseDto? Manufacturer { get; set; }
        public PublisherResponseDto? Publisher { get; set; }
        public List<ProductPhotoResponseDto> Photos { get; set; } = new();
    }

    public class ProductPhotoResponseDto
    {
        public int Id { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}