using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class PaymentRepository : BaseRepository<Payments>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Payments>> GetOrderPaymentsAsync(int orderId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(p => p.Order)
                .Where(p => p.OrderId == orderId)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<Payments?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId, cancellationToken);
        }

        public async Task<IEnumerable<Payments>> GetPaymentsByStatusAsync(string status, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(p => p.Order)
                .Where(p => p.PaymentStatus == status)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<decimal> GetTotalPaymentsAsync(int orderId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(p => p.OrderId == orderId && p.PaymentStatus == "Success")
                .SumAsync(p => p.Amount, cancellationToken);
        }

        public async Task<IEnumerable<Payments>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(p => p.Order)
                .Where(p => p.CreatedDate >= startDate && p.CreatedDate <= endDate)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync(cancellationToken);
        }
    }
}