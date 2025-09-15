using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IBlogLikeRepository
    {
        Task<BlogLikes?> GetByBlogAndUserAsync(int blogId, string userId);
        Task<BlogLikes> CreateAsync(BlogLikes like);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int blogId, string userId);
        Task<int> GetLikeCountByBlogIdAsync(int blogId);
        Task<List<BlogLikes>> GetByUserIdAsync(string userId, int page = 1, int pageSize = 10);
    }
}
