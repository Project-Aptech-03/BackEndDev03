using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Category
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Category code cannot be empty.")]
        [StringLength(1, MinimumLength = 1, ErrorMessage = "Category code must be exactly 1 letter.")]
        [RegularExpression(@"^[A-Z]{1}$", ErrorMessage = "Category code must be a single uppercase letter.")]
        public string CategoryCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category name cannot be empty.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string CategoryName { get; set; } = string.Empty;

        public List<string>? SubCategoryNames { get; set; } = null;
    }
}
