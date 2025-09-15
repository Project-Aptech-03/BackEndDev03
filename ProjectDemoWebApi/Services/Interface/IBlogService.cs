using ProjectDemoWebApi.DTOs.Blog;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IBlogService
    {
        Task<BlogResponseDto?> GetByIdAsync(int id, string? currentUserId = null);
        Task<BlogResponseDto?> GetBySlugAsync(string slug, string? currentUserId = null);
        Task<PagedResultDto<BlogListResponseDto>> GetPagedAsync(BlogQueryDto query, string? currentUserId = null);
        Task<List<BlogListResponseDto>> GetFeaturedAsync(int count = 5, string? currentUserId = null);
        Task<List<BlogListResponseDto>> GetRecentAsync(int count = 5, string? currentUserId = null);
        Task<List<BlogListResponseDto>> GetByAuthorAsync(string authorId, int page = 1, int pageSize = 10, string? currentUserId = null);
        Task<List<BlogListResponseDto>> GetByCategoryAsync(int categoryId, int page = 1, int pageSize = 10, string? currentUserId = null);
        Task<BlogResponseDto> CreateAsync(CreateBlogDto dto, string authorId);
        Task<BlogResponseDto> UpdateAsync(int id, UpdateBlogDto dto, string currentUserId);
        Task<bool> DeleteAsync(int id, string currentUserId);
        Task<bool> LikeBlogAsync(int blogId, string userId);
        Task<bool> UnlikeBlogAsync(int blogId, string userId);
        Task IncrementViewCountAsync(int id);
        Task<List<BlogListResponseDto>> SearchAsync(string searchTerm, int page = 1, int pageSize = 10, string? currentUserId = null);
    }
}
