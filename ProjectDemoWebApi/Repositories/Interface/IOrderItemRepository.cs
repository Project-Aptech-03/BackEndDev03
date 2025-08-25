using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IOrderItemRepository : IBaseRepository<OrderItems>
    {
        Task<IEnumerable<OrderItems>> GetOrderItemsAsync(int orderId, CancellationToken cancellationToken = default);
        Task<IEnumerable<OrderItems>> GetOrderItemsWithProductsAsync(int orderId, CancellationToken cancellationToken = default);
        Task<decimal> GetOrderTotalAsync(int orderId, CancellationToken cancellationToken = default);
        Task<IEnumerable<OrderItems>> GetProductOrderHistoryAsync(int productId, CancellationToken cancellationToken = default);
    }
}