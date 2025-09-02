using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IProductsRepository : IBaseRepository<Products>
    {
        Task<Products?> GetByProductCodeAsync(string productCode, CancellationToken cancellationToken = default);
        Task<Products?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Products>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Products>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Products>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Products>> GetByManufacturerAsync(int manufacturerId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Products>> GetByPublisherAsync(int publisherId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Products>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<bool> IsProductCodeExistsAsync(string productCode, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Products>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default);
        Task UpdateStockAsync(int productId, int newStock, CancellationToken cancellationToken = default);
        Task<Products?> GetByIdNoTrackingAsync(int id, CancellationToken cancellationToken = default);
    }
}