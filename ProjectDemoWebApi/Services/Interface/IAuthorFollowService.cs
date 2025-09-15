using ProjectDemoWebApi.DTOs.Blog;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IAuthorFollowService
    {
        Task<List<AuthorFollowDto>> GetFollowersAsync(string authorId, int page = 1, int pageSize = 10, string? currentUserId = null);
        Task<List<AuthorFollowDto>> GetFollowingAsync(string followerId, int page = 1, int pageSize = 10, string? currentUserId = null);
        Task<bool> FollowAuthorAsync(string authorId, string followerId);
        Task<bool> UnfollowAuthorAsync(string authorId, string followerId);
        Task<bool> IsFollowingAsync(string authorId, string followerId);
        Task<List<AuthorFollowDto>> GetSuggestedAuthorsAsync(string currentUserId, int count = 5);
    }
}
