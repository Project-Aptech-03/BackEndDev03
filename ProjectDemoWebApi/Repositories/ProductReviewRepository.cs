using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class ProductReviewRepository : BaseRepository<ProductReviews>, IProductReviewRepository
    {
        public ProductReviewRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ProductReviews?> GetReviewWithDetailsAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .Include(r => r.ReviewImages.Where(ri => ri.IsActive))
                .Include(r => r.ReviewReplies.Where(rr => rr.IsActive))
                    .ThenInclude(rr => rr.User)
                .Include(r => r.ReviewReplies.Where(rr => rr.IsActive))
                    .ThenInclude(rr => rr.ChildReplies.Where(cr => cr.IsActive))
                        .ThenInclude(cr => cr.User)
                .FirstOrDefaultAsync(r => r.Id == reviewId && r.IsActive, cancellationToken);
        }

        public async Task<IEnumerable<ProductReviews>> GetReviewsByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(r => r.Customer)
                .Include(r => r.ReviewImages.Where(ri => ri.IsActive))
                .Include(r => r.ReviewReplies.Where(rr => rr.IsActive))
                    .ThenInclude(rr => rr.User)
                .Where(r => r.ProductId == productId && r.IsActive )
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ProductReviews>> GetReviewsByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .Include(r => r.ReviewImages.Where(ri => ri.IsActive))
                .Include(r => r.ReviewReplies.Where(rr => rr.IsActive))
                    .ThenInclude(rr => rr.User)
                .Where(r => r.OrderId == orderId && r.IsActive)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ProductReviews>> GetReviewsByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(r => r.Product)
                .Include(r => r.ReviewImages.Where(ri => ri.IsActive))
                .Include(r => r.ReviewReplies.Where(rr => rr.IsActive))
                    .ThenInclude(rr => rr.User)
                .Where(r => r.CustomerId == customerId && r.IsActive)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ProductReviews>> GetPendingReviewsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .Where(r => !r.IsApproved && r.IsActive)
                .OrderBy(r => r.ReviewDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<double> GetAverageRatingByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(r => r.ProductId == productId && r.IsActive )
                .AverageAsync(r => (double?)r.Rating ?? 0, cancellationToken);
        }

        public async Task<int> GetReviewCountByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .CountAsync(r => r.ProductId == productId && r.IsActive && r.IsApproved, cancellationToken);
        }

        public async Task<bool> HasCustomerReviewedProductAsync(string customerId, int productId, int orderId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AnyAsync(r => r.CustomerId == customerId &&
                              r.ProductId == productId &&
                              r.OrderId == orderId &&
                              r.IsActive, cancellationToken);
        }
    }
}