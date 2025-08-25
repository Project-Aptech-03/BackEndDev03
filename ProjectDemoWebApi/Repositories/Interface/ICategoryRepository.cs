using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface ICategoryRepository : IBaseRepository<Categories>
    {
        Task<Categories?> GetByCategoryCodeAsync(string categoryCode, CancellationToken cancellationToken = default);
        Task<IEnumerable<Categories>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);
        Task<bool> IsCategoryCodeExistsAsync(string categoryCode, int? excludeId = null, CancellationToken cancellationToken = default);
    }
}