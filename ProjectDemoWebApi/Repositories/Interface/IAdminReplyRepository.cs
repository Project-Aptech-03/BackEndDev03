using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IAdminReplyRepository : IBaseRepository<AdminReplies>
    {
        Task<IEnumerable<AdminReplies>> GetQueryRepliesAsync(int queryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<AdminReplies>> GetAdminRepliesAsync(string adminId, CancellationToken cancellationToken = default);
        Task<AdminReplies?> GetLatestReplyAsync(int queryId, CancellationToken cancellationToken = default);
    }
}