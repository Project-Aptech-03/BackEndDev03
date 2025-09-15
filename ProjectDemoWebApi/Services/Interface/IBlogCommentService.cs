using ProjectDemoWebApi.DTOs.Blog;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IBlogCommentService
    {
        Task<List<CommentResponseDto>> GetByBlogIdAsync(int blogId, string? currentUserId = null);
        Task<CommentResponseDto> CreateAsync(int blogId, CreateCommentDto dto, string userId);
        Task<CommentResponseDto> UpdateAsync(int id, UpdateCommentDto dto, string currentUserId);
        Task<bool> DeleteAsync(int id, string currentUserId);
        Task<bool> LikeCommentAsync(int commentId, string userId);
        Task<bool> UnlikeCommentAsync(int commentId, string userId);
    }
}
