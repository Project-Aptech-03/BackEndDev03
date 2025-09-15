using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IManufacturerRepository : IBaseRepository<Manufacturers>
    {
        Task<Manufacturers?> GetByManufacturerCodeAsync(string manufacturerCode, CancellationToken cancellationToken = default);
        Task<IEnumerable<Manufacturers>> GetActiveManufacturersAsync(CancellationToken cancellationToken = default);
        Task<bool> IsManufacturerCodeExistsAsync(string manufacturerCode, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<List<string>> GetCodesByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
    }
}