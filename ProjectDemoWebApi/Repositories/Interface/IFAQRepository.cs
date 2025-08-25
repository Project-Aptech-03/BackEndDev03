using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IFAQRepository : IBaseRepository<FAQ>
    {
        Task<IEnumerable<FAQ>> GetActiveFAQsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<FAQ>> GetFAQsByOrderAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<FAQ>> SearchFAQsAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task ReorderFAQsAsync(List<(int Id, int SortOrder)> faqOrders, CancellationToken cancellationToken = default);
    }
}