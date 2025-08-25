using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Category
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Category code cannot be empty.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Category code must be exactly 2 characters.")]
        [RegularExpression(@"^[A-Z]\d$", ErrorMessage = "Category code must follow format: one letter + one digit (e.g., B1, M2).")]
        public string CategoryCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category name cannot be empty.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string CategoryName { get; set; } = string.Empty;
    }
}