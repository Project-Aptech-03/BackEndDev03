using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IPaymentRepository : IBaseRepository<Payments>
    {
        Task<IEnumerable<Payments>> GetOrderPaymentsAsync(int orderId, CancellationToken cancellationToken = default);
        Task<Payments?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Payments>> GetPaymentsByStatusAsync(string status, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalPaymentsAsync(int orderId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Payments>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    }
}