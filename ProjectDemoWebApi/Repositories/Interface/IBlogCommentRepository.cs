using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IBlogCommentRepository
    {
        Task<BlogComments?> GetByIdAsync(int id);
        Task<List<BlogComments>> GetByBlogIdAsync(int blogId);
        Task<List<BlogComments>> GetByUserIdAsync(string userId, int page = 1, int pageSize = 10);
        Task<BlogComments> CreateAsync(BlogComments comment);
        Task<BlogComments> UpdateAsync(BlogComments comment);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<int> GetCommentCountByBlogIdAsync(int blogId);
        Task<List<BlogComments>> GetRepliesAsync(int parentCommentId);
    }
}
