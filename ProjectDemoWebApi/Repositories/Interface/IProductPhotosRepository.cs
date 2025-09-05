using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IProductPhotosRepository : IBaseRepository<ProductPhotos>
    {
        Task<IEnumerable<ProductPhotos>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);
        Task<ProductPhotos?> GetByIdAndProductIdAsync(int id, int productId, CancellationToken cancellationToken = default);
        Task<bool> DeleteByIdAndProductIdAsync(int id, int productId, CancellationToken cancellationToken = default);
    }
}