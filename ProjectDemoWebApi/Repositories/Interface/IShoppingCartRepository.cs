using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IShoppingCartRepository : IBaseRepository<ShoppingCart>
    {
        Task<IEnumerable<ShoppingCart>> GetUserCartAsync(string userId, CancellationToken cancellationToken = default);
        Task<ShoppingCart?> GetUserCartItemAsync(string userId, int productId, CancellationToken cancellationToken = default);
        Task<decimal> GetCartTotalAsync(string userId, CancellationToken cancellationToken = default);
        Task<int> GetCartItemCountAsync(string userId, CancellationToken cancellationToken = default);
        Task ClearUserCartAsync(string userId, CancellationToken cancellationToken = default);
        Task RemoveExpiredCartItemsAsync(DateTime cutoffDate, CancellationToken cancellationToken = default);
    }
}