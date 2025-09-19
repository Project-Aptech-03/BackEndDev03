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

        public async Task<IEnumerable<Orders>> GetAllOrdersWithDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(o => o.Customer)
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
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<Orders?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(o => o.Customer)
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
                .Include(o => o.Customer)
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
                .Include(o => o.Customer)
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
                .Where(o => o.CustomerId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Orders>> GetOrdersByStatusAsync(string status, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsSplitQuery()
                .Where(o => EF.Functions.Collate(o.OrderStatus, "SQL_Latin1_General_CP1_CI_AS") == EF.Functions.Collate(status, "SQL_Latin1_General_CP1_CI_AS"))
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Orders>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(o => o.Customer)
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
                .Include(o => o.Customer)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsSplitQuery()
                .Where(o => pendingStatuses.Any(status => EF.Functions.Collate(o.OrderStatus, "SQL_Latin1_General_CP1_CI_AS") == EF.Functions.Collate(status, "SQL_Latin1_General_CP1_CI_AS")))
                .OrderBy(o => o.OrderDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<decimal> GetTotalSalesAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable()
                .Where(o => EF.Functions.Collate(o.OrderStatus, "SQL_Latin1_General_CP1_CI_AS") == EF.Functions.Collate("Delivered", "SQL_Latin1_General_CP1_CI_AS"));

            if (startDate.HasValue)
                query = query.Where(o => o.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(o => o.OrderDate <= endDate.Value);

            return await query.SumAsync(o => o.TotalAmount, cancellationToken);
        }

        public async Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken = default)
        {
            // Use a database transaction to ensure thread-safety and prevent race conditions
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            
            try
            {
                // Get the latest order number with ORD prefix within the transaction
                var latestOrder = await _dbSet
                    .Where(o => o.OrderNumber.StartsWith("ORD"))
                    .OrderByDescending(o => o.OrderNumber)
                    .Select(o => o.OrderNumber)
                    .FirstOrDefaultAsync(cancellationToken);

                int nextNumber = 1;

                if (!string.IsNullOrEmpty(latestOrder) && latestOrder.Length >= 8) // ORD + 5 digits
                {
                    // Extract the number part from ORD00001 format
                    var numberPart = latestOrder.Substring(3); // Remove "ORD" prefix
                    if (int.TryParse(numberPart, out var currentNumber))
                    {
                        nextNumber = currentNumber + 1;
                    }
                }

                // Format as ORD00001, ORD00002, etc.
                var newOrderNumber = $"ORD{nextNumber:D5}";
                
                // Double-check that this order number doesn't exist (extra safety)
                var exists = await IsOrderNumberExistsAsync(newOrderNumber, cancellationToken);
                if (exists)
                {
                    // If it exists (very rare race condition), try a few more numbers
                    for (int i = 1; i <= 10; i++)
                    {
                        newOrderNumber = $"ORD{(nextNumber + i):D5}";
                        exists = await IsOrderNumberExistsAsync(newOrderNumber, cancellationToken);
                        if (!exists) break;
                    }
                    
                    if (exists)
                    {
                        throw new InvalidOperationException("Unable to generate unique order number after multiple attempts");
                    }
                }

                await transaction.CommitAsync(cancellationToken);
                return newOrderNumber;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<bool> IsOrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(o => o.OrderNumber == orderNumber, cancellationToken);
        }
    }
}