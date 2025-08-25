using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class ProductReturnRepository : BaseRepository<ProductReturns>, IProductReturnRepository
    {
        public ProductReturnRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<ProductReturns?> GetByReturnNumberAsync(string returnNumber, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(pr => pr.Order)
                    .ThenInclude(o => o!.OrderItems)
                        .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(pr => pr.ReturnNumber == returnNumber, cancellationToken);
        }

        public async Task<IEnumerable<ProductReturns>> GetOrderReturnsAsync(int orderId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(pr => pr.Order)
                .Where(pr => pr.OrderId == orderId)
                .OrderByDescending(pr => pr.ReturnDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ProductReturns>> GetCustomerReturnsAsync(string customerId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(pr => pr.Order)
                    .ThenInclude(o => o!.OrderItems)
                        .ThenInclude(oi => oi.Product)
                .Where(pr => pr.CustomerId == customerId)
                .OrderByDescending(pr => pr.ReturnDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ProductReturns>> GetReturnsByStatusAsync(string status, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(pr => pr.Order)
                    .ThenInclude(o => o!.OrderItems)
                        .ThenInclude(oi => oi.Product)
                .Where(pr => pr.Status == status)
                .OrderByDescending(pr => pr.ReturnDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<string> GenerateReturnNumberAsync(CancellationToken cancellationToken = default)
        {
            string returnNumber;
            bool exists;
            
            do
            {
                // Generate R + 7 digits
                var random = new Random();
                var number = random.Next(1000000, 9999999);
                returnNumber = $"R{number}";
                
                exists = await IsReturnNumberExistsAsync(returnNumber, cancellationToken);
            } 
            while (exists);

            return returnNumber;
        }

        public async Task<bool> IsReturnNumberExistsAsync(string returnNumber, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(pr => pr.ReturnNumber == returnNumber, cancellationToken);
        }

        public async Task<decimal> GetTotalRefundsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable()
                .Where(pr => pr.Status == "Completed");

            if (startDate.HasValue)
                query = query.Where(pr => pr.ReturnDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(pr => pr.ReturnDate <= endDate.Value);

            return await query.SumAsync(pr => pr.RefundAmount, cancellationToken);
        }
    }
}