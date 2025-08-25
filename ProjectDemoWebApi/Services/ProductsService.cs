using AutoMapper;
using ProjectDemoWebApi.DTOs.Products;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly IPublisherRepository _publisherRepository;
        private readonly IStockMovementRepository _stockMovementRepository;
        private readonly IMapper _mapper;

        public ProductsService(
            IProductsRepository productsRepository,
            ICategoryRepository categoryRepository,
            IManufacturerRepository manufacturerRepository,
            IPublisherRepository publisherRepository,
            IStockMovementRepository stockMovementRepository,
            IMapper mapper)
        {
            _productsRepository = productsRepository ?? throw new ArgumentNullException(nameof(productsRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _manufacturerRepository = manufacturerRepository ?? throw new ArgumentNullException(nameof(manufacturerRepository));
            _publisherRepository = publisherRepository ?? throw new ArgumentNullException(nameof(publisherRepository));
            _stockMovementRepository = stockMovementRepository ?? throw new ArgumentNullException(nameof(stockMovementRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ApiResponse<IEnumerable<ProductsResponseDto>>> GetAllProductsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var products = await _productsRepository.GetAllWithDetailsAsync(cancellationToken);
                var productDtos = _mapper.Map<IEnumerable<ProductsResponseDto>>(products);
                
                return ApiResponse<IEnumerable<ProductsResponseDto>>.Ok(
                    productDtos, 
                    "Products retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductsResponseDto>>.Fail(
                    "An error occurred while retrieving products.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductsResponseDto>>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var products = await _productsRepository.GetActiveProductsAsync(cancellationToken);
                var productDtos = _mapper.Map<IEnumerable<ProductsResponseDto>>(products);
                
                return ApiResponse<IEnumerable<ProductsResponseDto>>.Ok(
                    productDtos, 
                    "Active products retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductsResponseDto>>.Fail(
                    "An error occurred while retrieving active products.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<ProductsResponseDto?>> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return ApiResponse<ProductsResponseDto?>.Fail(
                        "Invalid product ID.", 
                        null, 
                        400
                    );
                }

                var product = await _productsRepository.GetByIdWithDetailsAsync(id, cancellationToken);
                
                if (product == null)
                {
                    return ApiResponse<ProductsResponseDto?>.Fail(
                        "Product not found.", 
                        null, 
                        404
                    );
                }

                var productDto = _mapper.Map<ProductsResponseDto>(product);
                
                return ApiResponse<ProductsResponseDto?>.Ok(
                    productDto, 
                    "Product retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductsResponseDto?>.Fail(
                    "An error occurred while retrieving the product.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<ProductsResponseDto?>> GetProductByCodeAsync(string productCode, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productCode))
                {
                    return ApiResponse<ProductsResponseDto?>.Fail(
                        "Product code cannot be empty.", 
                        null, 
                        400
                    );
                }

                var product = await _productsRepository.GetByProductCodeAsync(productCode, cancellationToken);
                
                if (product == null)
                {
                    return ApiResponse<ProductsResponseDto?>.Fail(
                        "Product not found.", 
                        null, 
                        404
                    );
                }

                var productDto = _mapper.Map<ProductsResponseDto>(product);
                
                return ApiResponse<ProductsResponseDto?>.Ok(
                    productDto, 
                    "Product retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductsResponseDto?>.Fail(
                    "An error occurred while retrieving the product.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductsResponseDto>>> GetProductsByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (categoryId <= 0)
                {
                    return ApiResponse<IEnumerable<ProductsResponseDto>>.Fail(
                        "Invalid category ID.", 
                        null, 
                        400
                    );
                }

                var products = await _productsRepository.GetByCategoryAsync(categoryId, cancellationToken);
                var productDtos = _mapper.Map<IEnumerable<ProductsResponseDto>>(products);
                
                return ApiResponse<IEnumerable<ProductsResponseDto>>.Ok(
                    productDtos, 
                    "Products retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductsResponseDto>>.Fail(
                    "An error occurred while retrieving products by category.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductsResponseDto>>> GetProductsByManufacturerAsync(int manufacturerId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (manufacturerId <= 0)
                {
                    return ApiResponse<IEnumerable<ProductsResponseDto>>.Fail(
                        "Invalid manufacturer ID.", 
                        null, 
                        400
                    );
                }

                var products = await _productsRepository.GetByManufacturerAsync(manufacturerId, cancellationToken);
                var productDtos = _mapper.Map<IEnumerable<ProductsResponseDto>>(products);
                
                return ApiResponse<IEnumerable<ProductsResponseDto>>.Ok(
                    productDtos, 
                    "Products retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductsResponseDto>>.Fail(
                    "An error occurred while retrieving products by manufacturer.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductsResponseDto>>> GetProductsByPublisherAsync(int publisherId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (publisherId <= 0)
                {
                    return ApiResponse<IEnumerable<ProductsResponseDto>>.Fail(
                        "Invalid publisher ID.", 
                        null, 
                        400
                    );
                }

                var products = await _productsRepository.GetByPublisherAsync(publisherId, cancellationToken);
                var productDtos = _mapper.Map<IEnumerable<ProductsResponseDto>>(products);
                
                return ApiResponse<IEnumerable<ProductsResponseDto>>.Ok(
                    productDtos, 
                    "Products retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductsResponseDto>>.Fail(
                    "An error occurred while retrieving products by publisher.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductsResponseDto>>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return ApiResponse<IEnumerable<ProductsResponseDto>>.Fail(
                        "Search term cannot be empty.", 
                        null, 
                        400
                    );
                }

                var products = await _productsRepository.SearchProductsAsync(searchTerm, cancellationToken);
                var productDtos = _mapper.Map<IEnumerable<ProductsResponseDto>>(products);
                
                return ApiResponse<IEnumerable<ProductsResponseDto>>.Ok(
                    productDtos, 
                    "Products search completed successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductsResponseDto>>.Fail(
                    "An error occurred while searching products.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductsResponseDto>>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                if (threshold < 0)
                {
                    return ApiResponse<IEnumerable<ProductsResponseDto>>.Fail(
                        "Threshold cannot be negative.", 
                        null, 
                        400
                    );
                }

                var products = await _productsRepository.GetLowStockProductsAsync(threshold, cancellationToken);
                var productDtos = _mapper.Map<IEnumerable<ProductsResponseDto>>(products);
                
                return ApiResponse<IEnumerable<ProductsResponseDto>>.Ok(
                    productDtos, 
                    "Low stock products retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductsResponseDto>>.Fail(
                    "An error occurred while retrieving low stock products.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<ProductsResponseDto>> CreateProductAsync(CreateProductsDto createProductDto, CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate related entities
                var category = await _categoryRepository.GetByIdAsync(createProductDto.CategoryId, cancellationToken);
                if (category == null)
                {
                    return ApiResponse<ProductsResponseDto>.Fail(
                        "Invalid category ID.", 
                        null, 
                        400
                    );
                }

                var manufacturer = await _manufacturerRepository.GetByIdAsync(createProductDto.ManufacturerId, cancellationToken);
                if (manufacturer == null)
                {
                    return ApiResponse<ProductsResponseDto>.Fail(
                        "Invalid manufacturer ID.", 
                        null, 
                        400
                    );
                }

                if (createProductDto.PublisherId.HasValue)
                {
                    var publisher = await _publisherRepository.GetByIdAsync(createProductDto.PublisherId.Value, cancellationToken);
                    if (publisher == null)
                    {
                        return ApiResponse<ProductsResponseDto>.Fail(
                            "Invalid publisher ID.", 
                            null, 
                            400
                        );
                    }
                }

                // Check if product code already exists
                var codeExists = await _productsRepository.IsProductCodeExistsAsync(createProductDto.ProductCode, null, cancellationToken);
                if (codeExists)
                {
                    return ApiResponse<ProductsResponseDto>.Fail(
                        "Product code already exists.", 
                        null, 
                        409
                    );
                }

                var product = _mapper.Map<Products>(createProductDto);
                product.CreatedDate = DateTime.UtcNow;

                await _productsRepository.AddAsync(product, cancellationToken);
                await _productsRepository.SaveChangesAsync(cancellationToken);

                // Add initial stock movement
                await _stockMovementRepository.AddStockMovementAsync(
                    product.Id, 
                    product.StockQuantity, 
                    0, 
                    product.StockQuantity, 
                    "INITIAL", 
                    null, 
                    0, 
                    "Initial stock", 
                    "System", 
                    cancellationToken);
                
                await _stockMovementRepository.SaveChangesAsync(cancellationToken);

                var productDto = _mapper.Map<ProductsResponseDto>(product);
                
                return ApiResponse<ProductsResponseDto>.Ok(
                    productDto, 
                    "Product created successfully.", 
                    201
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductsResponseDto>.Fail(
                    "An error occurred while creating the product.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<ProductsResponseDto?>> UpdateProductAsync(int id, UpdateProductsDto updateProductDto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return ApiResponse<ProductsResponseDto?>.Fail(
                        "Invalid product ID.", 
                        null, 
                        400
                    );
                }

                var product = await _productsRepository.GetByIdAsync(id, cancellationToken);
                
                if (product == null)
                {
                    return ApiResponse<ProductsResponseDto?>.Fail(
                        "Product not found.", 
                        null, 
                        404
                    );
                }

                var previousStock = product.StockQuantity;

                // Update only provided fields
                if (!string.IsNullOrWhiteSpace(updateProductDto.ProductName))
                    product.ProductName = updateProductDto.ProductName;
                
                if (updateProductDto.CategoryId.HasValue)
                {
                    var category = await _categoryRepository.GetByIdAsync(updateProductDto.CategoryId.Value, cancellationToken);
                    if (category == null)
                    {
                        return ApiResponse<ProductsResponseDto?>.Fail(
                            "Invalid category ID.", 
                            null, 
                            400
                        );
                    }
                    product.CategoryId = updateProductDto.CategoryId.Value;
                }

                if (updateProductDto.ManufacturerId.HasValue)
                {
                    var manufacturer = await _manufacturerRepository.GetByIdAsync(updateProductDto.ManufacturerId.Value, cancellationToken);
                    if (manufacturer == null)
                    {
                        return ApiResponse<ProductsResponseDto?>.Fail(
                            "Invalid manufacturer ID.", 
                            null, 
                            400
                        );
                    }
                    product.ManufacturerId = updateProductDto.ManufacturerId.Value;
                }

                if (updateProductDto.PublisherId.HasValue)
                {
                    var publisher = await _publisherRepository.GetByIdAsync(updateProductDto.PublisherId.Value, cancellationToken);
                    if (publisher == null)
                    {
                        return ApiResponse<ProductsResponseDto?>.Fail(
                            "Invalid publisher ID.", 
                            null, 
                            400
                        );
                    }
                    product.PublisherId = updateProductDto.PublisherId.Value;
                }

                if (!string.IsNullOrWhiteSpace(updateProductDto.Description))
                    product.Description = updateProductDto.Description;
                
                if (!string.IsNullOrWhiteSpace(updateProductDto.Author))
                    product.Author = updateProductDto.Author;
                
                if (!string.IsNullOrWhiteSpace(updateProductDto.ProductType))
                    product.ProductType = updateProductDto.ProductType;
                
                if (updateProductDto.Pages.HasValue)
                    product.Pages = updateProductDto.Pages.Value;
                
                if (!string.IsNullOrWhiteSpace(updateProductDto.Dimensions))
                    product.Dimensions = updateProductDto.Dimensions;
                
                if (updateProductDto.Weight.HasValue)
                    product.Weight = updateProductDto.Weight.Value;
                
                if (updateProductDto.Price.HasValue)
                    product.Price = updateProductDto.Price.Value;
                
                if (updateProductDto.StockQuantity.HasValue)
                {
                    product.StockQuantity = updateProductDto.StockQuantity.Value;
                    
                    // Add stock movement record
                    await _stockMovementRepository.AddStockMovementAsync(
                        product.Id, 
                        updateProductDto.StockQuantity.Value - previousStock, 
                        previousStock, 
                        updateProductDto.StockQuantity.Value, 
                        "ADJUSTMENT", 
                        null, 
                        0, 
                        "Stock adjustment", 
                        "System", 
                        cancellationToken);
                }
                
                if (updateProductDto.IsActive.HasValue)
                    product.IsActive = updateProductDto.IsActive.Value;

                _productsRepository.Update(product);
                await _productsRepository.SaveChangesAsync(cancellationToken);
                await _stockMovementRepository.SaveChangesAsync(cancellationToken);

                var productDto = _mapper.Map<ProductsResponseDto>(product);
                
                return ApiResponse<ProductsResponseDto?>.Ok(
                    productDto, 
                    "Product updated successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductsResponseDto?>.Fail(
                    "An error occurred while updating the product.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<bool>> DeleteProductAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return ApiResponse<bool>.Fail(
                        "Invalid product ID.", 
                        false, 
                        400
                    );
                }

                var product = await _productsRepository.GetByIdAsync(id, cancellationToken);
                
                if (product == null)
                {
                    return ApiResponse<bool>.Fail(
                        "Product not found.", 
                        false, 
                        404
                    );
                }

                _productsRepository.Delete(product);
                await _productsRepository.SaveChangesAsync(cancellationToken);
                
                return ApiResponse<bool>.Ok(
                    true, 
                    "Product deleted successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(
                    "An error occurred while deleting the product.", 
                    false, 
                    500
                );
            }
        }

        public async Task<ApiResponse<bool>> UpdateStockAsync(int productId, int newStock, CancellationToken cancellationToken = default)
        {
            try
            {
                if (productId <= 0)
                {
                    return ApiResponse<bool>.Fail(
                        "Invalid product ID.", 
                        false, 
                        400
                    );
                }

                if (newStock < 0)
                {
                    return ApiResponse<bool>.Fail(
                        "Stock quantity cannot be negative.", 
                        false, 
                        400
                    );
                }

                var product = await _productsRepository.GetByIdAsync(productId, cancellationToken);
                
                if (product == null)
                {
                    return ApiResponse<bool>.Fail(
                        "Product not found.", 
                        false, 
                        404
                    );
                }

                var previousStock = product.StockQuantity;
                await _productsRepository.UpdateStockAsync(productId, newStock, cancellationToken);
                
                // Add stock movement record
                await _stockMovementRepository.AddStockMovementAsync(
                    productId, 
                    newStock - previousStock, 
                    previousStock, 
                    newStock, 
                    "ADJUSTMENT", 
                    null, 
                    0, 
                    "Stock update", 
                    "System", 
                    cancellationToken);

                await _productsRepository.SaveChangesAsync(cancellationToken);
                await _stockMovementRepository.SaveChangesAsync(cancellationToken);
                
                return ApiResponse<bool>.Ok(
                    true, 
                    "Stock updated successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(
                    "An error occurred while updating stock.", 
                    false, 
                    500
                );
            }
        }

        public async Task<ApiResponse<bool>> IsProductCodeExistsAsync(string productCode, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productCode))
                {
                    return ApiResponse<bool>.Fail(
                        "Product code cannot be empty.", 
                        false, 
                        400
                    );
                }

                var exists = await _productsRepository.IsProductCodeExistsAsync(productCode, excludeId, cancellationToken);
                
                return ApiResponse<bool>.Ok(
                    exists, 
                    "Product code existence checked successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(
                    "An error occurred while checking product code existence.", 
                    false, 
                    500
                );
            }
        }

        public async Task<ApiResponse<(IEnumerable<ProductsResponseDto> Products, int TotalCount)>> GetProductsPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (pageNumber <= 0)
                    pageNumber = 1;
                
                if (pageSize <= 0 || pageSize > 100)
                    pageSize = 20;

                var (products, totalCount) = await _productsRepository.GetPagedAsync(pageNumber, pageSize, p => p.IsActive, cancellationToken);
                var productDtos = _mapper.Map<IEnumerable<ProductsResponseDto>>(products);
                
                return ApiResponse<(IEnumerable<ProductsResponseDto> Products, int TotalCount)>.Ok(
                    (productDtos, totalCount), 
                    "Products retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<(IEnumerable<ProductsResponseDto> Products, int TotalCount)>.Fail(
                    "An error occurred while retrieving products.", 
                    (Enumerable.Empty<ProductsResponseDto>(), 0), 
                    500
                );
            }
        }
    }
}