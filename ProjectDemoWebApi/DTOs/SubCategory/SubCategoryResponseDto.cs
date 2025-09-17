namespace ProjectDemoWebApi.DTOs.SubCategory
{
    public class SubCategoryResponseDto
    {
        public int Id { get; set; }
        public string SubCategoryCode { get; set; } = string.Empty;
        public string SubCategoryName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
