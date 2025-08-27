using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class OrderRepository : BaseRepository<Orders>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Orders?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(o => o.DeliveryAddress)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .AsSplitQuery() // Optimize multiple includes
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, cancellationToken);
        }

        public async Task<Orders?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(o => o.DeliveryAddress)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p!.Category)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p!.Manufacturer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p!.ProductPhotos.Where(ph => ph.IsActive))
                .Include(o => o.Payments)
                .AsSplitQuery()
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Orders>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(o => o.DeliveryAddress)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsSplitQuery()
                .Where(o => o.CustomerId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Orders>> GetOrdersByStatusAsync(string status, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(o => o.DeliveryAddress)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsSplitQuery()
                .Where(o => o.OrderStatus == status)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Orders>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(o => o.DeliveryAddress)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsSplitQuery()
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Orders>> GetPendingOrdersAsync(CancellationToken cancellationToken = default)
        {
            var pendingStatuses = new[] { "Pending", "Confirmed" };
            
            return await _dbSet.AsNoTracking()
                .Include(o => o.DeliveryAddress)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsSplitQuery()
                .Where(o => pendingStatuses.Contains(o.OrderStatus))
                .OrderBy(o => o.OrderDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<decimal> GetTotalSalesAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable()
                .Where(o => o.OrderStatus == "Delivered");

            if (startDate.HasValue)
                query = query.Where(o => o.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(o => o.OrderDate <= endDate.Value);

            return await query.SumAsync(o => o.TotalAmount, cancellationToken);
        }

        public async Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken = default)
        {
            string orderNumber;
            bool exists;
            var maxAttempts = 100; // Prevent infinite loop
            var attempts = 0;
            
            do
            {
                attempts++;
                if (attempts > maxAttempts)
                    throw new InvalidOperationException("Unable to generate unique order number after maximum attempts");
                    
                // Generate 8-digit number with better randomness
                var random = new Random(Guid.NewGuid().GetHashCode());
                var number = random.Next(10000000, 99999999);
                orderNumber = number.ToString();
                
                exists = await IsOrderNumberExistsAsync(orderNumber, cancellationToken);
            } 
            while (exists);

            return orderNumber;
        }

        public async Task<bool> IsOrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(o => o.OrderNumber == orderNumber, cancellationToken);
        }
    }
}