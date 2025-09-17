namespace ProjectDemoWebApi.DTOs.SubCategory
{
    public class UpdateSubCategoriesDto
    {
        public int? Id { get; set; } 
        public string SubCategoryName { get; set; } = null!;
        public bool? IsActive { get; set; }
    }
}
