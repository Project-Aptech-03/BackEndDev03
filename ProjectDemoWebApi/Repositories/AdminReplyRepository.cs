using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class AdminReplyRepository : BaseRepository<AdminReplies>, IAdminReplyRepository
    {
        public AdminReplyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AdminReplies>> GetQueryRepliesAsync(int queryId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(ar => ar.Query)
                .Where(ar => ar.QueryId == queryId)
                .OrderBy(ar => ar.ReplyDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<AdminReplies>> GetAdminRepliesAsync(string adminId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(ar => ar.Query)
                .Where(ar => ar.AdminId == adminId)
                .OrderByDescending(ar => ar.ReplyDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<AdminReplies?> GetLatestReplyAsync(int queryId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(ar => ar.Query)
                .Where(ar => ar.QueryId == queryId)
                .OrderByDescending(ar => ar.ReplyDate)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}