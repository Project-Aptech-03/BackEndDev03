using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class ShoppingCartRepository : BaseRepository<ShoppingCart>, IShoppingCartRepository
    {
        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ShoppingCart>> GetUserCartAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(sc => sc.Product)
                    .ThenInclude(p => p!.Category)
                .Include(sc => sc.Product)
                    .ThenInclude(p => p!.Manufacturer)
                .Include(sc => sc.Product)
                    .ThenInclude(p => p!.ProductPhotos.Where(ph => ph.IsActive))
                .AsSplitQuery()
                .Where(sc => sc.UserId == userId)
                .OrderBy(sc => sc.AddedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<ShoppingCart?> GetUserCartItemAsync(string userId, int productId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(
                sc => sc.UserId == userId && sc.ProductId == productId, 
                cancellationToken);
        }

        public async Task<decimal> GetCartTotalAsync(string userId, CancellationToken cancellationToken = default)
        {
            var total = await _dbSet
                .Where(sc => sc.UserId == userId)
                .SumAsync(sc => sc.UnitPrice * sc.Quantity, cancellationToken);
                
            return total;
        }

        public async Task<int> GetCartItemCountAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(sc => sc.UserId == userId)
                .SumAsync(sc => sc.Quantity, cancellationToken);
        }

        public async Task ClearUserCartAsync(string userId, CancellationToken cancellationToken = default)
        {
            // Use bulk delete for better performance
            await _dbSet
                .Where(sc => sc.UserId == userId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task RemoveExpiredCartItemsAsync(DateTime cutoffDate, CancellationToken cancellationToken = default)
        {
            // Use bulk delete for better performance
            await _dbSet
                .Where(sc => sc.UpdatedDate < cutoffDate)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task RemoveProductsFromCartAsync(string userId, IEnumerable<int> productIds, CancellationToken cancellationToken = default)
        {
            // Use bulk delete for better performance
            await _dbSet
                .Where(sc => sc.UserId == userId && productIds.Contains(sc.ProductId))
                .ExecuteDeleteAsync(cancellationToken);
        }
    }
}