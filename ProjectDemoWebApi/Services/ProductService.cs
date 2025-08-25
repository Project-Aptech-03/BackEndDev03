using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IGoogleCloudStorageService _googleCloudStorageService;

        public ProductService(IProductRepository productRepository, IGoogleCloudStorageService googleCloudStorageService)
        {
            _productRepository = productRepository;
            _googleCloudStorageService = googleCloudStorageService;
        }

        public async Task<IEnumerable<Products>> GetAllProductsAsync(CancellationToken cancellationToken = default)
        {
            return await _productRepository.GetAllAsync(cancellationToken);
        }

        public async Task<Products?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _productRepository.GetByIdAsync(id, cancellationToken);
        }

        public async Task<Products> CreateProductAsync(Products products, CancellationToken cancellationToken = default)
        {
            await _productRepository.AddAsync(products, cancellationToken);
            await _productRepository.SaveChangesAsync(cancellationToken);
            return products;
        }

        public async Task<Products?> UpdateProductAsync(Products products, CancellationToken cancellationToken = default)
        {
            var existing = await _productRepository.GetByIdAsync(products.Id, cancellationToken);
            if (existing == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(products.ProductName))
            {
                existing.ProductName = products.ProductName;
            }

            if (!string.IsNullOrEmpty(products.Description))
            {
                existing.Description = products.Description;
            }

            if (products.Price != 0)
            {
                existing.Price = products.Price;
            }

            if (products.CategoryId != 0)
            {
                existing.CategoryId = products.CategoryId;
            }

            if (products.ManufacturerId != 0)
            {
                existing.ManufacturerId = products.ManufacturerId;
            }

            if (products.PublisherId.HasValue)
            {
                existing.PublisherId = products.PublisherId;
            }

            if (!string.IsNullOrEmpty(products.Author))
            {
                existing.Author = products.Author;
            }

            if (!string.IsNullOrEmpty(products.ProductType))
            {
                existing.ProductType = products.ProductType;
            }

            if (products.Pages.HasValue)
            {
                existing.Pages = products.Pages;
            }

            if (!string.IsNullOrEmpty(products.Dimensions))
            {
                existing.Dimensions = products.Dimensions;
            }

            if (products.Weight.HasValue)
            {
                existing.Weight = products.Weight;
            }

            if (products.StockQuantity >= 0)
            {
                existing.StockQuantity = products.StockQuantity;
            }

            existing.IsActive = products.IsActive;

            _productRepository.Update(existing);
            await _productRepository.SaveChangesAsync(cancellationToken);
            return existing;
        }

        public async Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null) return false;

            // Delete associated product photos from cloud storage
            if (product.ProductPhotos != null && product.ProductPhotos.Any())
            {
                foreach (var photo in product.ProductPhotos)
                {
                    if (!string.IsNullOrEmpty(photo.PhotoUrl))
                    {
                        await _googleCloudStorageService.DeleteFileAsync(photo.PhotoUrl, cancellationToken);
                    }
                }
            }

            _productRepository.Delete(product);
            return await _productRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task AddProductImagesAsync(List<ProductPhotos> images, CancellationToken cancellationToken)
        {
            await _productRepository.AddProductImagesAsync(images, cancellationToken);
            await _productRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
