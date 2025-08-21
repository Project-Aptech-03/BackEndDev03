using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Product
{
    public class UpdateProductDto
    {
        public string? Name { get; set; } = string.Empty;

        
        public decimal? Price { get; set; }

        public string? Description { get; set; } = string.Empty;

        public List<IFormFile>? NewImages { get; set; } = new();
        public List<int>? ExistingImageIds { get; set; } = new();
        public int? MainImageId { get; set; }
    }


}
