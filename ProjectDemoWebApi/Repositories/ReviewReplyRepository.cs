using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class ReviewReplyRepository : BaseRepository<ReviewReplies>, IReviewReplyRepository
    {
        public ReviewReplyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ReviewReplies>> GetRepliesByReviewIdAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(rr => rr.User)
                .Where(rr => rr.ReviewId == reviewId && rr.IsActive && rr.ParentReplyId == null)
                .OrderBy(rr => rr.ReplyDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ReviewReplies>> GetRepliesWithChildrenAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(rr => rr.User)
                .Include(rr => rr.ChildReplies.Where(cr => cr.IsActive))
                    .ThenInclude(cr => cr.User)
                .Where(rr => rr.ReviewId == reviewId && rr.IsActive && rr.ParentReplyId == null)
                .OrderBy(rr => rr.ReplyDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<ReviewReplies?> GetReplyWithDetailsAsync(int replyId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(rr => rr.User)
                .Include(rr => rr.ChildReplies.Where(cr => cr.IsActive))
                    .ThenInclude(cr => cr.User)
                .Include(rr => rr.ParentReply)
                .FirstOrDefaultAsync(rr => rr.Id == replyId && rr.IsActive, cancellationToken);
        }
    }
}