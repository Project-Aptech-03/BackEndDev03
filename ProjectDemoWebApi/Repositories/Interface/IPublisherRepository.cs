using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IPublisherRepository : IBaseRepository<Publishers>
    {
        Task<IEnumerable<Publishers>> GetActivePublishersAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Publishers>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    }
}