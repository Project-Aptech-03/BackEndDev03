using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class OrderItemRepository : BaseRepository<OrderItems>, IOrderItemRepository
    {
        public OrderItemRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OrderItems>> GetOrderItemsAsync(int orderId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<OrderItems>> GetOrderItemsWithProductsAsync(int orderId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(oi => oi.Product)
                    .ThenInclude(p => p!.Category)
                .Include(oi => oi.Product)
                    .ThenInclude(p => p!.Manufacturer)
                .Include(oi => oi.Product)
                    .ThenInclude(p => p!.ProductPhotos.Where(ph => ph.IsActive))
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync(cancellationToken);
        }

        public async Task<decimal> GetOrderTotalAsync(int orderId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(oi => oi.OrderId == orderId)
                .SumAsync(oi => oi.TotalPrice, cancellationToken);
        }

        public async Task<IEnumerable<OrderItems>> GetProductOrderHistoryAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(oi => oi.Order)
                .Where(oi => oi.ProductId == productId)
                .OrderByDescending(oi => oi.Order!.OrderDate)
                .ToListAsync(cancellationToken);
        }
    }
}