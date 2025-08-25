using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IProductRepository
    {
        Task<IEnumerable<Products>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Products?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task AddAsync(Products products, CancellationToken cancellationToken = default);
        void Update(Products products);
        void Delete(Products products);
        Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task AddProductImagesAsync(List<ProductPhotos> images, CancellationToken cancellationToken);

    }
}
