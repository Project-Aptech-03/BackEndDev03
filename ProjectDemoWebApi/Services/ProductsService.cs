using AutoMapper;
using ProjectDemoWebApi.DTOs.Products;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace ProjectDemoWebApi.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly IPublisherRepository _publisherRepository;
        private readonly IMapper _mapper;

        public ProductsService(
            IProductsRepository productsRepository,
            ICategoryRepository categoryRepository,
            IManufacturerRepository manufacturerRepository,
            IPublisherRepository publisherRepository,
            IMapper mapper)
        {
            _productsRepository = productsRepository ?? throw new ArgumentNullException(nameof(productsRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _manufacturerRepository = manufacturerRepository ?? throw new ArgumentNullException(nameof(manufacturerRepository));
            _publisherRepository = publisherRepository ?? throw new ArgumentNullException(nameof(publisherRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ApiResponse<IEnumerable<ProductsResponseDto>>> GetAllProductsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var products = await _productsRepository.GetAllWithDetailsAsync(cancellationToken);
                var productDtos = _mapper.Map<IEnumerable<ProductsResponseDto>>(products);
                
                return ApiResponse<IEnumerable<ProductsResponseDto>>.Ok(productDtos, "Products retrieved successfully.", 200);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductsResponseDto>>.Fail("An error occurred while retrieving products.", null, 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductsResponseDto>>> GetProductsByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (categoryId <= 0)
                    return ApiResponse<IEnumerable<ProductsResponseDto>>.Fail("Invalid category ID.", null, 400);

                var products = await _productsRepository.GetByCategoryAsync(categoryId, cancellationToken);
                var productDtos = _mapper.Map<IEnumerable<ProductsResponseDto>>(products);
                
                return ApiResponse<IEnumerable<ProductsResponseDto>>.Ok(productDtos, "Products retrieved successfully.", 200);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductsResponseDto>>.Fail("An error occurred while retrieving products by category.", null, 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductsResponseDto>>> GetProductsByManufacturerAsync(int manufacturerId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (manufacturerId <= 0)
                    return ApiResponse<IEnumerable<ProductsResponseDto>>.Fail("Invalid manufacturer ID.", null, 400);

                var products = await _productsRepository.GetByManufacturerAsync(manufacturerId, cancellationToken);
                var productDtos = _mapper.Map<IEnumerable<ProductsResponseDto>>(products);
                
                return ApiResponse<IEnumerable<ProductsResponseDto>>.Ok(productDtos, "Products retrieved successfully.", 200);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductsResponseDto>>.Fail("An error occurred while retrieving products by manufacturer.", null, 500);
            }
        }

        public async Task<ApiResponse<ProductsResponseDto>> CreateProductAsync(CreateProductDto createProductDto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (createProductDto == null)
                    return ApiResponse<ProductsResponseDto>.Fail("Product data is required.", null, 400);

                // Validate ProductCode
                if (string.IsNullOrWhiteSpace(createProductDto.ProductCode) || createProductDto.ProductCode.Length != 7)
                    return ApiResponse<ProductsResponseDto>.Fail("Product code must be exactly 7 characters.", null, 400);

                if (!System.Text.RegularExpressions.Regex.IsMatch(createProductDto.ProductCode, @"^[A-Z0-9]{7}$"))
                    return ApiResponse<ProductsResponseDto>.Fail("Product code must contain only uppercase letters and numbers.", null, 400);

                // Validate references
                if (await _categoryRepository.GetByIdAsync(createProductDto.CategoryId, cancellationToken) == null)
                    return ApiResponse<ProductsResponseDto>.Fail("Invalid category ID.", null, 400);

                if (await _manufacturerRepository.GetByIdAsync(createProductDto.ManufacturerId, cancellationToken) == null)
                    return ApiResponse<ProductsResponseDto>.Fail("Invalid manufacturer ID.", null, 400);

                if (createProductDto.PublisherId.HasValue && 
                    await _publisherRepository.GetByIdAsync(createProductDto.PublisherId.Value, cancellationToken) == null)
                    return ApiResponse<ProductsResponseDto>.Fail("Invalid publisher ID.", null, 400);

                // Check if product code exists
                if (await _productsRepository.IsProductCodeExistsAsync(createProductDto.ProductCode, null, cancellationToken))
                    return ApiResponse<ProductsResponseDto>.Fail($"Product code '{createProductDto.ProductCode}' already exists.", null, 409);

                // Create product
                var product = _mapper.Map<Products>(createProductDto);
                product.CreatedDate = DateTime.UtcNow;

                await _productsRepository.AddAsync(product, cancellationToken);
                await _productsRepository.SaveChangesAsync(cancellationToken);

                var createdProduct = await _productsRepository.GetByIdNoTrackingAsync(product.Id, cancellationToken);
                var productDto = _mapper.Map<ProductsResponseDto>(createdProduct);
                
                return ApiResponse<ProductsResponseDto>.Ok(productDto, "Product created successfully.", 201);
            }
            catch (DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                
                if (innerMessage.Contains("UNIQUE") || innerMessage.Contains("duplicate") || innerMessage.Contains("product_code"))
                    return ApiResponse<ProductsResponseDto>.Fail($"Product code '{createProductDto.ProductCode}' already exists.", null, 409);
                if (innerMessage.Contains("FOREIGN KEY") || innerMessage.Contains("foreign key"))
                    return ApiResponse<ProductsResponseDto>.Fail("Invalid reference to category, manufacturer, or publisher.", null, 400);
                if (innerMessage.Contains("truncated"))
                    return ApiResponse<ProductsResponseDto>.Fail("One or more field values exceed maximum length.", null, 400);
                
                return ApiResponse<ProductsResponseDto>.Fail($"Database error: {innerMessage}", null, 500);
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductsResponseDto>.Fail("An error occurred while creating the product.", null, 500);
            }
        }

        public async Task<ApiResponse<ProductsResponseDto?>> UpdateProductAsync(int id, UpdateProductDto updateProductDto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0 || updateProductDto == null)
                    return ApiResponse<ProductsResponseDto?>.Fail("Invalid input data.", null, 400);

                var product = await _productsRepository.GetByIdAsync(id, cancellationToken);
                if (product == null)
                    return ApiResponse<ProductsResponseDto?>.Fail("Product not found.", null, 404);

                // Update fields
                if (!string.IsNullOrWhiteSpace(updateProductDto.ProductName))
                    product.ProductName = updateProductDto.ProductName;
                
                if (updateProductDto.CategoryId.HasValue)
                {
                    if (await _categoryRepository.GetByIdAsync(updateProductDto.CategoryId.Value, cancellationToken) == null)
                        return ApiResponse<ProductsResponseDto?>.Fail("Invalid category ID.", null, 400);
                    product.CategoryId = updateProductDto.CategoryId.Value;
                }

                if (updateProductDto.ManufacturerId.HasValue)
                {
                    if (await _manufacturerRepository.GetByIdAsync(updateProductDto.ManufacturerId.Value, cancellationToken) == null)
                        return ApiResponse<ProductsResponseDto?>.Fail("Invalid manufacturer ID.", null, 400);
                    product.ManufacturerId = updateProductDto.ManufacturerId.Value;
                }

                if (updateProductDto.PublisherId.HasValue)
                {
                    if (await _publisherRepository.GetByIdAsync(updateProductDto.PublisherId.Value, cancellationToken) == null)
                        return ApiResponse<ProductsResponseDto?>.Fail("Invalid publisher ID.", null, 400);
                    product.PublisherId = updateProductDto.PublisherId.Value;
                }

                if (!string.IsNullOrWhiteSpace(updateProductDto.Description)) product.Description = updateProductDto.Description;
                if (!string.IsNullOrWhiteSpace(updateProductDto.Author)) product.Author = updateProductDto.Author;
                if (!string.IsNullOrWhiteSpace(updateProductDto.ProductType)) product.ProductType = updateProductDto.ProductType;
                if (updateProductDto.Pages.HasValue) product.Pages = updateProductDto.Pages.Value;
                if (!string.IsNullOrWhiteSpace(updateProductDto.Dimensions)) product.Dimensions = updateProductDto.Dimensions;
                if (updateProductDto.Weight.HasValue) product.Weight = updateProductDto.Weight.Value;
                if (updateProductDto.Price.HasValue) product.Price = updateProductDto.Price.Value;
                if (updateProductDto.StockQuantity.HasValue) product.StockQuantity = updateProductDto.StockQuantity.Value;
                if (updateProductDto.IsActive.HasValue) product.IsActive = updateProductDto.IsActive.Value;

                _productsRepository.Update(product);
                await _productsRepository.SaveChangesAsync(cancellationToken);

                var updatedProduct = await _productsRepository.GetByIdNoTrackingAsync(product.Id, cancellationToken);
                var productDto = _mapper.Map<ProductsResponseDto>(updatedProduct);
                
                return ApiResponse<ProductsResponseDto?>.Ok(productDto, "Product updated successfully.", 200);
            }
            catch (DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return ApiResponse<ProductsResponseDto?>.Fail($"Database error: {innerMessage}", null, 500);
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductsResponseDto?>.Fail("An error occurred while updating the product.", null, 500);
            }
        }

        public async Task<ApiResponse<bool>> UpdateStockAsync(int productId, int newStock, CancellationToken cancellationToken = default)
        {
            try
            {
                if (productId <= 0 || newStock < 0)
                    return ApiResponse<bool>.Fail("Invalid input data.", false, 400);

                var product = await _productsRepository.GetByIdAsync(productId, cancellationToken);
                if (product == null)
                    return ApiResponse<bool>.Fail("Product not found.", false, 404);

                if (product.StockQuantity == newStock)
                    return ApiResponse<bool>.Ok(true, "Stock is already at the specified level.", 200);

                await _productsRepository.UpdateStockAsync(productId, newStock, cancellationToken);
                await _productsRepository.SaveChangesAsync(cancellationToken);
                
                return ApiResponse<bool>.Ok(true, "Stock updated successfully.", 200);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail("An error occurred while updating stock.", false, 500);
            }
        }

        public async Task<ApiResponse<bool>> DeleteProductAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return ApiResponse<bool>.Fail("Invalid product ID.", false, 400);

                var product = await _productsRepository.GetByIdAsync(id, cancellationToken);
                if (product == null)
                    return ApiResponse<bool>.Fail("Product not found.", false, 404);

                product.IsActive = false;
                _productsRepository.Update(product);
                await _productsRepository.SaveChangesAsync(cancellationToken);
                
                return ApiResponse<bool>.Ok(true, "Product deactivated successfully.", 200);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail("An error occurred while deactivating the product.", false, 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductsResponseDto>>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                if (threshold < 0)
                    return ApiResponse<IEnumerable<ProductsResponseDto>>.Fail("Threshold cannot be negative.", null, 400);

                var products = await _productsRepository.GetLowStockProductsAsync(threshold, cancellationToken);
                var productDtos = _mapper.Map<IEnumerable<ProductsResponseDto>>(products);
                
                return ApiResponse<IEnumerable<ProductsResponseDto>>.Ok(productDtos, "Low stock products retrieved successfully.", 200);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductsResponseDto>>.Fail("An error occurred while retrieving low stock products.", null, 500);
            }
        }
    }
}