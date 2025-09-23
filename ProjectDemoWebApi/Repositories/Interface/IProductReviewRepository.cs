using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IProductReviewRepository : IBaseRepository<ProductReviews>
    {
        Task<ProductReviews?> GetReviewWithDetailsAsync(int reviewId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductReviews>> GetReviewsByProductIdAsync(int productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductReviews>> GetReviewsByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductReviews>> GetReviewsByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductReviews>> GetPendingReviewsAsync(CancellationToken cancellationToken = default);
        Task<double> GetAverageRatingByProductIdAsync(int productId, CancellationToken cancellationToken = default);
        Task<int> GetReviewCountByProductIdAsync(int productId, CancellationToken cancellationToken = default);
        Task<bool> HasCustomerReviewedProductAsync(string customerId, int productId, int orderId, CancellationToken cancellationToken = default);
    }
}