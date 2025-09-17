using ProjectDemoWebApi.DTOs.SubCategory;

namespace ProjectDemoWebApi.DTOs.Category
{
    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string CategoryCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ProductCount { get; set; }

        public List<SubCategoryResponseDto>? SubCategories { get; set; } = null;
    }
}