using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IReviewReplyRepository : IBaseRepository<ReviewReplies>
    {
        Task<IEnumerable<ReviewReplies>> GetRepliesByReviewIdAsync(int reviewId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ReviewReplies>> GetRepliesWithChildrenAsync(int reviewId, CancellationToken cancellationToken = default);
        Task<ReviewReplies?> GetReplyWithDetailsAsync(int replyId, CancellationToken cancellationToken = default);
    }
}