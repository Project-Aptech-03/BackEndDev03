using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync(CancellationToken cancellationToken = default)
        {
            return await _productRepository.GetAllAsync(cancellationToken);
        }

        public async Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _productRepository.GetByIdAsync(id, cancellationToken);
        }

        public async Task<Product> CreateProductAsync(Product product, CancellationToken cancellationToken = default)
        {
            await _productRepository.AddAsync(product, cancellationToken);
            await _productRepository.SaveChangesAsync(cancellationToken);
            return product;
        }

        public async Task<Product?> UpdateProductAsync(Product product, CancellationToken cancellationToken = default)
        {
            var existing = await _productRepository.GetByIdAsync(product.Id, cancellationToken);
            if (existing == null)
            {
                return null;
            }

            // Update các trường (nếu có logic nào cần làm trước khi save)
            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;

            _productRepository.Update(existing);
            await _productRepository.SaveChangesAsync(cancellationToken);
            return existing;
        }

        public async Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null) return false;

            _productRepository.Delete(product);
            return await _productRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
