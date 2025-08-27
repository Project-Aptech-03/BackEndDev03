using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class CustomerQueryRepository : BaseRepository<CustomerQueries>, ICustomerQueryRepository
    {
        public CustomerQueryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CustomerQueries>> GetCustomerQueriesAsync(string? customerId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking()
                .Include(cq => cq.AdminReplies)
                .AsQueryable();

            if (!string.IsNullOrEmpty(customerId))
                query = query.Where(cq => cq.CustomerId == customerId);

            return await query
                .OrderByDescending(cq => cq.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<CustomerQueries>> GetQueriesByStatusAsync(string status, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(cq => cq.AdminReplies)
                .Where(cq => cq.Status == status)
                .OrderByDescending(cq => cq.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<CustomerQueries?> GetQueryWithRepliesAsync(int queryId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(cq => cq.AdminReplies.OrderBy(ar => ar.ReplyDate))
                .FirstOrDefaultAsync(cq => cq.Id == queryId, cancellationToken);
        }

        public async Task<IEnumerable<CustomerQueries>> GetOpenQueriesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(cq => cq.AdminReplies)
                .Where(cq => cq.Status == "Open")
                .OrderBy(cq => cq.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetPendingQueriesCountAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.CountAsync(cq => cq.Status == "Open", cancellationToken);
        }
    }
}