using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class StockMovementRepository : BaseRepository<StockMovements>, IStockMovementRepository
    {
        public StockMovementRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<StockMovements>> GetProductStockHistoryAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(sm => sm.Product)
                .Where(sm => sm.ProductId == productId)
                .OrderByDescending(sm => sm.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<StockMovements>> GetStockMovementsByTypeAsync(string referenceType, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(sm => sm.Product)
                .Where(sm => sm.ReferenceType == referenceType)
                .OrderByDescending(sm => sm.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<StockMovements>> GetStockMovementsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(sm => sm.Product)
                .Where(sm => sm.CreatedDate >= startDate && sm.CreatedDate <= endDate)
                .OrderByDescending(sm => sm.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<StockMovements?> GetLatestStockMovementAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(sm => sm.Product)
                .Where(sm => sm.ProductId == productId)
                .OrderByDescending(sm => sm.CreatedDate)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddStockMovementAsync(int productId, int quantity, int previousStock, int newStock, string referenceType, int? referenceId, decimal unitCost, string? reason, string createdBy, CancellationToken cancellationToken = default)
        {
            var stockMovement = new StockMovements
            {
                ProductId = productId,
                Quantity = quantity,
                PreviousStock = previousStock,
                NewStock = newStock,
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                UnitCost = unitCost,
                Reason = reason,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow
            };

            await _dbSet.AddAsync(stockMovement, cancellationToken);
        }
    }
}