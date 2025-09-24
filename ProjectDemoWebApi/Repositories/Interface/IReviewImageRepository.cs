using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IReviewImageRepository : IBaseRepository<ReviewImages>
    {
        Task<IEnumerable<ReviewImages>> GetImagesByReviewIdAsync(int reviewId, CancellationToken cancellationToken = default);
        Task DeleteImagesByReviewIdAsync(int reviewId, CancellationToken cancellationToken = default);
    }
}