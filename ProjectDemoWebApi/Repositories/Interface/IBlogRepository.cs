using ProjectDemoWebApi.DTOs.Blog;
using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IBlogRepository
    {
        Task<Blogs?> GetByIdAsync(int id);
        Task<Blogs?> GetBySlugAsync(string slug);
        Task<PagedResultDto<Blogs>> GetPagedAsync(BlogQueryDto query);
        Task<List<Blogs>> GetFeaturedAsync(int count = 5);
        Task<List<Blogs>> GetRecentAsync(int count = 5);
        Task<List<Blogs>> GetByAuthorAsync(string authorId, int page = 1, int pageSize = 10);
        Task<List<Blogs>> GetByCategoryAsync(int categoryId, int page = 1, int pageSize = 10);
        Task<Blogs> CreateAsync(Blogs blog);
        Task<Blogs> UpdateAsync(Blogs blog);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
        Task IncrementViewCountAsync(int id);
        Task<int> GetTotalCountAsync();
        Task<List<Blogs>> SearchAsync(string searchTerm, int page = 1, int pageSize = 10);
    }
}
