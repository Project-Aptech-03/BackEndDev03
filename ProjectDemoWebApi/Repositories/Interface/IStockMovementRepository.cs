using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IStockMovementRepository : IBaseRepository<StockMovements>
    {
        Task<IEnumerable<StockMovements>> GetProductStockHistoryAsync(int productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<StockMovements>> GetStockMovementsByTypeAsync(string referenceType, CancellationToken cancellationToken = default);
        Task<IEnumerable<StockMovements>> GetStockMovementsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<StockMovements?> GetLatestStockMovementAsync(int productId, CancellationToken cancellationToken = default);
        Task AddStockMovementAsync(int productId, int quantity, int previousStock, int newStock, string referenceType, int? referenceId, decimal unitCost, string? reason, string createdBy, CancellationToken cancellationToken = default);
    }
}