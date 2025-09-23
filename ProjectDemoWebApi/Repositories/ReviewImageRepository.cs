using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class ReviewImageRepository : BaseRepository<ReviewImages>, IReviewImageRepository
    {
        public ReviewImageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ReviewImages>> GetImagesByReviewIdAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(ri => ri.ReviewId == reviewId && ri.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task DeleteImagesByReviewIdAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            var images = await _dbSet
                .Where(ri => ri.ReviewId == reviewId)
                .ToListAsync(cancellationToken);

            foreach (var image in images)
            {
                image.IsActive = false;
            }

            _dbSet.UpdateRange(images);
        }
    }
}