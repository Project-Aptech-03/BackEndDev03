using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface ICategoryRepository : IBaseRepository<Categories>
    {
        Task<Categories?> GetByCategoryCodeAsync(string categoryCode, CancellationToken cancellationToken = default);
        Task<(IEnumerable<Categories> Items, int TotalCount)> GetActiveCategoriesPagedAsync(
     int pageNumber = 1,
     int pageSize = 10,
     CancellationToken cancellationToken = default);
        Task<bool> IsCategoryCodeExistsAsync(string categoryCode, int? excludeId = null, CancellationToken cancellationToken = default);

        Task<IEnumerable<Categories>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Categories>> GetCategoriesByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
        Task<Categories?> GetByIdWithSubCategoriesAsync(int id, CancellationToken cancellationToken = default);
    }
}