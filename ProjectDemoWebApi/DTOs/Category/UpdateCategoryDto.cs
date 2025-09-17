using ProjectDemoWebApi.DTOs.SubCategory;
using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Category
{
    public class UpdateCategoryDto
    {
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string? CategoryName { get; set; }

        public bool? IsActive { get; set; }

        public List<UpdateSubCategoriesDto>? SubCategories { get; set; }
    }
}