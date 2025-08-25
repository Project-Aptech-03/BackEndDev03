using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IProductService
    {
        Task<IEnumerable<Products>> GetAllProductsAsync(CancellationToken cancellationToken = default);
        Task<Products?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Products> CreateProductAsync(Products products, CancellationToken cancellationToken = default);
        Task<Products?> UpdateProductAsync(Products products, CancellationToken cancellationToken = default);
        Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken = default);
        Task AddProductImagesAsync(List<ProductPhotos> images, CancellationToken cancellationToken);
    }
}
