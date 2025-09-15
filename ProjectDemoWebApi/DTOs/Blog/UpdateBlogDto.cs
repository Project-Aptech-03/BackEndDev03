using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Blog
{
    public class UpdateBlogDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Summary cannot exceed 500 characters")]
        public string? Summary { get; set; }

        [StringLength(500, ErrorMessage = "Featured image URL cannot exceed 500 characters")]
        public string? FeaturedImageUrl { get; set; }

        [StringLength(100, ErrorMessage = "Slug cannot exceed 100 characters")]
        public string? Slug { get; set; }

        public bool IsPublished { get; set; }

        public bool IsFeatured { get; set; }

        [Required(ErrorMessage = "Category ID is required")]
        public int CategoryId { get; set; }
    }
}
