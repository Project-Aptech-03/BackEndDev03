using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IAuthorFollowRepository
    {
        Task<AuthorFollows?> GetByFollowerAndAuthorAsync(string followerId, string authorId);
        Task<AuthorFollows> CreateAsync(AuthorFollows follow);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(string followerId, string authorId);
        Task<List<AuthorFollows>> GetFollowersAsync(string authorId, int page = 1, int pageSize = 10);
        Task<List<AuthorFollows>> GetFollowingAsync(string followerId, int page = 1, int pageSize = 10);
        Task<int> GetFollowerCountAsync(string authorId);
        Task<int> GetFollowingCountAsync(string followerId);
    }
}
