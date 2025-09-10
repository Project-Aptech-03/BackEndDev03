using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.DTOs.Products;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;
using System.Security.Claims;

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
        private readonly ILogger<ProductsService> _logger;
        private readonly IHttpContextAccessor _httpContectAccessor;
        private readonly IGoogleCloudStorageService _googleCloudStorageService;
        private readonly ApplicationDbContext _applicationDbContext;
        public ProductsService(
            IProductsRepository productsRepository,
            ICategoryRepository categoryRepository,
            IManufacturerRepository manufacturerRepository,
            IPublisherRepository publisherRepository,
            IStockMovementRepository stockMovementRepository,
            ILogger<ProductsService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IGoogleCloudStorageService googleCloudStorageService,
            ApplicationDbContext applicationDbContext)
        {
            _productsRepository = productsRepository ?? throw new ArgumentNullException(nameof(productsRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _manufacturerRepository = manufacturerRepository ?? throw new ArgumentNullException(nameof(manufacturerRepository));
            _publisherRepository = publisherRepository ?? throw new ArgumentNullException(nameof(publisherRepository));
            _stockMovementRepository = stockMovementRepository ?? throw new ArgumentNullException(nameof(stockMovementRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContectAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(_httpContectAccessor));
            _googleCloudStorageService = googleCloudStorageService;
            _applicationDbContext = applicationDbContext;
        }



        //getAllProduct
        public async Task<ApiResponse<PagedResponseDto<ProductsResponseDto>>> GetProductsPagedAsync(
         int pageNumber,
         int pageSize,
         CancellationToken cancellationToken = default)
        {
            try
            {
                if (pageNumber <= 0)
                    pageNumber = 1;

                if (pageSize <= 0 || pageSize > 100)
                    pageSize = 20;

                var (products, totalCount) = await _productsRepository
                    .GetPagedIncludeAsync(pageNumber, pageSize, p => p.IsActive, cancellationToken,
                        p => p.Category,
                        p => p.Manufacturer,
                        p => p.Publisher,
                        p => p.ProductPhotos);

                var productDtos = _mapper.Map<List<ProductsResponseDto>>(products);

                var response = new PagedResponseDto<ProductsResponseDto>
                {
                    Items = productDtos,
                    TotalCount = totalCount,
                    PageIndex = pageNumber,
                    PageSize = pageSize
                };

                return ApiResponse<PagedResponseDto<ProductsResponseDto>>.Ok(
                    response,
                    "Products retrieved successfully.",
                    200
                );
            }
            catch (Exception)
            {
                return ApiResponse<PagedResponseDto<ProductsResponseDto>>.Fail(
                    "An error occurred while retrieving products.",
                    new PagedResponseDto<ProductsResponseDto>(),
                    500
                );
            }
        }

       
        public async Task<ApiResponse<ProductsResponseDto>> CreateProductAsync(CreateProductsDto createProductDto, CancellationToken cancellationToken = default)
        {
            try
            {
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

                // 3. Validate Publisher (if provided)
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
                if (createProductDto.Photos != null && createProductDto.Photos.Any())
                {
                    var uploadedUrls = await _googleCloudStorageService.UploadFilesAsync(createProductDto.Photos, "products", cancellationToken);

                    foreach (var url in uploadedUrls)
                    {
                        product.ProductPhotos.Add(new ProductPhotos
                        {
                            PhotoUrl = url,
                            IsActive = true,
                            CreatedDate = DateTime.UtcNow
                        });
                    }
                }
                await _productsRepository.AddAsync(product, cancellationToken);
                await _productsRepository.SaveChangesAsync(cancellationToken);

                var createdByUserId = _httpContectAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(createdByUserId))
                {
                    return ApiResponse<ProductsResponseDto>.Fail(
                        "Unable to determine current user for stock movement.",
                        null,
                        500
                    );
                }

                await _stockMovementRepository.AddStockMovementAsync(
                    product.Id,
                    product.StockQuantity,
                    0,
                    product.StockQuantity,
                    "INITIAL",
                    null,
                    0,
                    "Initial stock",
                    createdByUserId,
                    cancellationToken
                );

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
                _logger.LogError(ex, "Error occurred while creating product");

                return ApiResponse<ProductsResponseDto>.Fail(
                    $"An error occurred while creating the product. Details: {ex.Message}",
                    null,
                    500
                );
            }
        }


  //      public async Task<ApiResponse<ProductsResponseDto?>> UpdateProductAsync(
  //int id,
  //UpdateProductsDto updateProductDto,
  //CancellationToken cancellationToken = default)
  //      {
  //          try
  //          {
  //              if (id <= 0)
  //                  return ApiResponse<ProductsResponseDto?>.Fail("Invalid product ID.", null, 400);

  //              var product = await _productsRepository.GetByIdWithDetailsAsync(id, cancellationToken);
  //              if (product == null)
  //                  return ApiResponse<ProductsResponseDto?>.Fail("Product not found.", null, 404);

  //              var previousStock = product.StockQuantity;

  //              product.ProductCode = updateProductDto.ProductCode ?? product.ProductCode;
  //              product.ProductName = updateProductDto.ProductName ?? product.ProductName;
  //              product.Description = updateProductDto.Description ?? product.Description;
  //              product.Author = updateProductDto.Author ?? product.Author;
  //              product.ProductType = updateProductDto.ProductType ?? product.ProductType;
  //              product.Pages = updateProductDto.Pages ?? product.Pages;
  //              product.Dimensions = updateProductDto.Dimensions ?? product.Dimensions;
  //              product.Weight = updateProductDto.Weight ?? product.Weight;
  //              product.Price = updateProductDto.Price ?? product.Price;
  //              product.IsActive = updateProductDto.IsActive ?? product.IsActive;

  //              // Update Category
  //              if (updateProductDto.CategoryId.HasValue)
  //              {
  //                  var category = await _categoryRepository.GetByIdAsync(updateProductDto.CategoryId.Value, cancellationToken);
  //                  if (category == null)
  //                      return ApiResponse<ProductsResponseDto?>.Fail("Invalid category ID.", null, 400);

  //                  product.CategoryId = category.Id;
  //                  product.Category = category;
  //              }

  //              // Update Manufacturer
  //              if (updateProductDto.ManufacturerId.HasValue)
  //              {
  //                  var manufacturer = await _manufacturerRepository.GetByIdAsync(updateProductDto.ManufacturerId.Value, cancellationToken);
  //                  if (manufacturer == null)
  //                      return ApiResponse<ProductsResponseDto?>.Fail("Invalid manufacturer ID.", null, 400);

  //                  product.ManufacturerId = manufacturer.Id;
  //                  product.Manufacturer = manufacturer;
  //              }

  //              // Update Publisher
  //              if (updateProductDto.PublisherId.HasValue)
  //              {
  //                  var publisher = await _publisherRepository.GetByIdAsync(updateProductDto.PublisherId.Value, cancellationToken);
  //                  if (publisher == null)
  //                      return ApiResponse<ProductsResponseDto?>.Fail("Invalid publisher ID.", null, 400);

  //                  product.PublisherId = publisher.Id;
  //                  product.Publisher = publisher;
  //              }

  //              // Update Stock
  //              if (updateProductDto.StockQuantity.HasValue && updateProductDto.StockQuantity.Value != previousStock)
  //              {
  //                  var quantityChange = updateProductDto.StockQuantity.Value - previousStock;
  //                  product.StockQuantity = updateProductDto.StockQuantity.Value;

  //                  await _stockMovementRepository.AddStockMovementAsync(
  //                      product.Id,
  //                      quantityChange,
  //                      previousStock,
  //                      product.StockQuantity,
  //                      "ADJUSTMENT",
  //                      null,
  //                      0,
  //                      "Stock adjustment",
  //                      "System",
  //                      cancellationToken);
  //              }

  //              // L?u entity v? stock movement
  //              await _productsRepository.SaveChangesAsync(cancellationToken);
  //              await _stockMovementRepository.SaveChangesAsync(cancellationToken);

  //              // Load l?i entity k?m relations ?? map DTO chu?n
  //              var updatedProduct = await _productsRepository.GetByIdWithDetailsAsync(product.Id, cancellationToken);
  //              var productDto = _mapper.Map<ProductsResponseDto>(updatedProduct);

  //              return ApiResponse<ProductsResponseDto?>.Ok(productDto, "Product updated successfully.", 200);
  //          }
  //          catch (Exception ex)
  //          {
  //              _logger.LogError(ex, "Error occurred while updating product");
  //              return ApiResponse<ProductsResponseDto?>.Fail("An error occurred while updating the product.", null, 500);
  //          }
  //      }


        // main
        public async Task<ApiResponse<ProductsResponseDto?>> UpdateProductAsync(
      int id,
      UpdateProductsDto updateProductDto,
      CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return ApiResponse<ProductsResponseDto?>.Fail("Invalid product ID.", null, 400);

                var product = await _productsRepository.GetByIdWithDetailsAsync(id, cancellationToken);
                if (product == null)
                    return ApiResponse<ProductsResponseDto?>.Fail("Product not found.", null, 404);

                var previousStock = product.StockQuantity;

                product.ProductName = updateProductDto.ProductName ?? product.ProductName;
                product.Description = updateProductDto.Description ?? product.Description;
                product.Author = updateProductDto.Author ?? product.Author;
                product.ProductType = updateProductDto.ProductType ?? product.ProductType;
                product.Pages = updateProductDto.Pages ?? product.Pages;
                product.Dimensions = updateProductDto.Dimensions ?? product.Dimensions;
                product.Weight = updateProductDto.Weight ?? product.Weight;
                product.Price = updateProductDto.Price ?? product.Price;
                product.IsActive = updateProductDto.IsActive ?? product.IsActive;

                if (updateProductDto.CategoryId.HasValue)
                {
                    var category = await _categoryRepository.GetByIdAsync(updateProductDto.CategoryId.Value, cancellationToken);
                    if (category == null)
                        return ApiResponse<ProductsResponseDto?>.Fail("Invalid category ID.", null, 400);

                    product.CategoryId = category.Id;
                    product.Category = category;
                }

                if (updateProductDto.ManufacturerId.HasValue)
                {
                    var manufacturer = await _manufacturerRepository.GetByIdAsync(updateProductDto.ManufacturerId.Value, cancellationToken);
                    if (manufacturer == null)
                        return ApiResponse<ProductsResponseDto?>.Fail("Invalid manufacturer ID.", null, 400);

                    product.ManufacturerId = manufacturer.Id;
                    product.Manufacturer = manufacturer;
                }

                if (updateProductDto.PublisherId.HasValue)
                {
                    var publisher = await _publisherRepository.GetByIdAsync(updateProductDto.PublisherId.Value, cancellationToken);
                    if (publisher == null)
                        return ApiResponse<ProductsResponseDto?>.Fail("Invalid publisher ID.", null, 400);

                    product.PublisherId = publisher.Id;
                    product.Publisher = publisher;
                }

                if (updateProductDto.StockQuantity.HasValue && updateProductDto.StockQuantity.Value != previousStock)
                {
                    var quantityChange = updateProductDto.StockQuantity.Value - previousStock;
                    product.StockQuantity = updateProductDto.StockQuantity.Value;

                    await _stockMovementRepository.AddStockMovementAsync(
                        product.Id,
                        quantityChange,
                        previousStock,
                        product.StockQuantity,
                        "ADJUSTMENT",
                        null,
                        0,
                        "Stock adjustment",
                        "System",
                        cancellationToken);
                }
                if (updateProductDto.Photos != null && updateProductDto.Photos.Any())
                {
                    var uploadedUrls = await _googleCloudStorageService.UploadFilesAsync(updateProductDto.Photos, "products", cancellationToken);
                    foreach (var url in uploadedUrls)
                    {
                        product.ProductPhotos.Add(new ProductPhotos
                        {
                            PhotoUrl = url,
                            IsActive = true,
                            CreatedDate = DateTime.UtcNow,

                        });

                    }
                }

                await _productsRepository.SaveChangesAsync(cancellationToken);
                await _stockMovementRepository.SaveChangesAsync(cancellationToken);

           
                var updatedProduct = await _productsRepository.GetByIdWithDetailsAsync(product.Id, cancellationToken);
                var productDto = _mapper.Map<ProductsResponseDto>(updatedProduct);

                return ApiResponse<ProductsResponseDto?>.Ok(productDto, "Product updated successfully.", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product");
                return ApiResponse<ProductsResponseDto?>.Fail("An error occurred while updating the product.", null, 500);
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





        //public async Task<ApiResponse<bool>> DeleteProductAsync(int id, CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        if (id <= 0)
        //        {
        //            return ApiResponse<bool>.Fail(
        //                "Invalid product ID.", 
        //                false, 
        //                400
        //            );
        //        }

        //        var product = await _productsRepository.GetByIdWithDetailsAsync(id, cancellationToken);

        //        if (product == null)
        //        {
        //            return ApiResponse<bool>.Fail(
        //                "Product not found.", 
        //                false, 
        //                404
        //            );
        //        }

        //        // Collect photo URLs before deletion
        //        var photoUrls = product.ProductPhotos?.Where(p => !string.IsNullOrWhiteSpace(p.PhotoUrl))
        //            .Select(p => p.PhotoUrl).ToList() ?? new List<string>();

        //        try
        //        {
        //            // Delete from database first
        //            _productsRepository.Delete(product);
        //            await _productsRepository.SaveChangesAsync(cancellationToken);

        //            // Only delete photos if database deletion was successful
        //            foreach (var photoUrl in photoUrls)
        //            {
        //                try
        //                {
        //                    await _googleCloudStorageService.DeleteFileAsync(photoUrl, cancellationToken);
        //                }
        //                catch (Exception ex)
        //                {
        //                    _logger.LogWarning(ex, "Failed to delete photo {PhotoUrl} for product {ProductId}", photoUrl, id);
        //                }
        //            }
        //        }
        //        catch (DbUpdateException ex)
        //        {
        //            _logger.LogError(ex, "Database error while deleting product {ProductId}", id);

        //            // Check if it's a foreign key constraint issue
        //            if (ex.InnerException?.Message?.Contains("REFERENCE") == true || 
        //                ex.InnerException?.Message?.Contains("FOREIGN KEY") == true)
        //            {
        //                return ApiResponse<bool>.Fail(
        //                    "Cannot delete product because it is referenced by other records (orders, cart items, etc.). Consider deactivating the product instead.",
        //                    false,
        //                    409
        //                );
        //            }

        //            return ApiResponse<bool>.Fail(
        //                "Database error occurred while deleting the product.",
        //                false,
        //                409
        //            );
        //        }

        //        return ApiResponse<bool>.Ok(
        //            true, 
        //            "Product deleted successfully.", 
        //            200
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Unexpected error while deleting product {ProductId}", id);
        //        return ApiResponse<bool>.Fail(
        //            "An error occurred while deleting the product.", 
        //            false, 
        //            500
        //        );
        //    }
        //}


        public async Task<ApiResponse<bool>> DeleteProductAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                return ApiResponse<bool>.Fail("Invalid product ID.", false, 400);

            try
            {
                // Load product kèm navigation
                var product = await _productsRepository.GetByIdWithDetailsAsync(id, cancellationToken);
                if (product == null)
                    return ApiResponse<bool>.Fail("Product not found.", false, 404);

                var photoUrls = product.ProductPhotos?
                    .Where(p => !string.IsNullOrWhiteSpace(p.PhotoUrl))
                    .Select(p => p.PhotoUrl)
                    .Distinct()
                    .ToList() ?? new List<string>();

                try
                {
                    // Xóa product (n?u cascade trong DB thì t?t c? con s? xóa theo)
                    _productsRepository.Delete(product);
                    await _productsRepository.SaveChangesAsync(cancellationToken);

                    // Xóa file trên cloud (n?u có)
                    foreach (var url in photoUrls)
                    {
                        try
                        {
                            await _googleCloudStorageService.DeleteFileAsync(url, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to delete photo {PhotoUrl} for product {ProductId}", url, id);
                        }
                    }

                    return ApiResponse<bool>.Ok(true, "Product deleted successfully.", 200);
                }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogWarning(dbEx, "Delete failed for product {ProductId}, fallback to deactivate.", id);

                    // N?u không th? xóa c?ng thì chuy?n sang deactivate
                    if (product.IsActive)
                    {
                        product.IsActive = false;
                        _productsRepository.Update(product);
                        await _productsRepository.SaveChangesAsync(cancellationToken);

                        return ApiResponse<bool>.Ok(true, "Product could not be deleted; deactivated instead.", 200);
                    }

                    return ApiResponse<bool>.Fail(
                        "Cannot delete or deactivate product due to DB constraints.",
                        false, 500);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting product {ProductId}", id);
                return ApiResponse<bool>.Fail(
                    $"An unexpected error occurred: {ex.InnerException?.Message ?? ex.Message}",
                    false, 500);
            }
        }





        public async Task<ApiResponse<int>> DeleteProductsAsync(List<int> ids, CancellationToken cancellationToken = default)
        {
            if (ids == null || ids.Count == 0)
                return ApiResponse<int>.Fail("No product ids provided.", 0, 400);

            var validIds = ids.Where(id => id > 0).Distinct().ToList();
            if (validIds.Count == 0)
                return ApiResponse<int>.Fail("No valid product ids provided.", 0, 400);

            try
            {
                int deletedCount = 0;
                int deactivatedCount = 0;
                var allPhotoUrls = new List<string>();

                foreach (var productId in validIds)
                {
                    var product = await _productsRepository.GetByIdWithDetailsAsync(productId, cancellationToken);
                    if (product == null) continue;

                    var photoUrls = product.ProductPhotos?
                        .Where(p => !string.IsNullOrWhiteSpace(p.PhotoUrl))
                        .Select(p => p.PhotoUrl)
                        .Distinct()
                        .ToList() ?? new List<string>();

                    try
                    {
                        // Th? xóa product
                        _productsRepository.Delete(product);
                        await _productsRepository.SaveChangesAsync(cancellationToken);
                        deletedCount++;

                        allPhotoUrls.AddRange(photoUrls);
                    }
                    catch (DbUpdateException dbEx)
                    {
                        _logger.LogWarning(dbEx, "Delete failed for product {ProductId}, fallback to deactivate.", productId);

                        // N?u xóa th?t b?i ? deactivate
                        if (product.IsActive)
                        {
                            product.IsActive = false;
                            _productsRepository.Update(product);
                            await _productsRepository.SaveChangesAsync(cancellationToken);
                            deactivatedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unexpected error while deleting product {ProductId}", productId);
                    }
                }

                if (deletedCount > 0 && allPhotoUrls.Count > 0)
                {
                    foreach (var url in allPhotoUrls.Distinct())
                    {
                        try
                        {
                            await _googleCloudStorageService.DeleteFileAsync(url, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to delete photo {PhotoUrl}", url);
                        }
                    }
                }

                var totalProcessed = deletedCount + deactivatedCount;
                string message = totalProcessed == 0
                    ? "No products were processed successfully."
                    : $"{deletedCount} products deleted, {deactivatedCount} products deactivated.";

                return ApiResponse<int>.Ok(totalProcessed, message, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatal error while deleting multiple products");
                return ApiResponse<int>.Fail("An unexpected error occurred while deleting products.", 0, 500);
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
     

    }
}