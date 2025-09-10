
using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IProductReturnRepository : IBaseRepository<ProductReturns>
    {
        Task<ProductReturns?> GetByReturnNumberAsync(string returnNumber, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductReturns>> GetOrderReturnsAsync(int orderId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductReturns>> GetCustomerReturnsAsync(string customerId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductReturns>> GetReturnsByStatusAsync(string status, CancellationToken cancellationToken = default);
        Task<string> GenerateReturnNumberAsync(CancellationToken cancellationToken = default);
        Task<bool> IsReturnNumberExistsAsync(string returnNumber, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalRefundsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    }
}
