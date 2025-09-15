namespace ProjectDemoWebApi.DTOs.Blog
{
    public class BlogQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public string? AuthorId { get; set; }
        public bool? IsPublished { get; set; } = true;
        public bool? IsFeatured { get; set; }
        public string? SortBy { get; set; } = "CreatedDate";
        public string? SortOrder { get; set; } = "desc"; 
    }
}
