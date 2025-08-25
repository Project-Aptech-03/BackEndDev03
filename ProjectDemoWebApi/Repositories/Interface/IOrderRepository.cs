using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IOrderRepository : IBaseRepository<Orders>
    {
        Task<Orders?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
        Task<Orders?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Orders>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Orders>> GetOrdersByStatusAsync(string status, CancellationToken cancellationToken = default);
        Task<IEnumerable<Orders>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<IEnumerable<Orders>> GetPendingOrdersAsync(CancellationToken cancellationToken = default);
        Task<decimal> GetTotalSalesAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
        Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken = default);
        Task<bool> IsOrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default);
    }
}