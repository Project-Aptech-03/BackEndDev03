using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface ICustomerQueryRepository : IBaseRepository<CustomerQueries>
    {
        Task<IEnumerable<CustomerQueries>> GetCustomerQueriesAsync(string? customerId = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<CustomerQueries>> GetQueriesByStatusAsync(string status, CancellationToken cancellationToken = default);
        Task<CustomerQueries?> GetQueryWithRepliesAsync(int queryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<CustomerQueries>> GetOpenQueriesAsync(CancellationToken cancellationToken = default);
        Task<int> GetPendingQueriesCountAsync(CancellationToken cancellationToken = default);
    }
}