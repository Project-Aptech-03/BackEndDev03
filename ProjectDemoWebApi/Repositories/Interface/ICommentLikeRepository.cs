using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface ICommentLikeRepository
    {
        Task<CommentLikes?> GetByCommentAndUserAsync(int commentId, string userId);
        Task<CommentLikes> CreateAsync(CommentLikes like);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int commentId, string userId);
        Task<int> GetLikeCountByCommentIdAsync(int commentId);
    }
}
