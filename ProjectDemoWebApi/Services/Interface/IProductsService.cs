using ProjectDemoWebApi.DTOs.Products;
using ProjectDemoWebApi.DTOs.Shared;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IProductsService
    {
        Task<ApiResponse<IEnumerable<ProductsResponseDto>>> GetAllProductsAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<IEnumerable<ProductsResponseDto>>> GetProductsByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<ApiResponse<IEnumerable<ProductsResponseDto>>> GetProductsByManufacturerAsync(int manufacturerId, CancellationToken cancellationToken = default);
        Task<ApiResponse<ProductsResponseDto>> CreateProductAsync(CreateProductDto createProductDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<ProductsResponseDto?>> UpdateProductAsync(int id, UpdateProductDto updateProductDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> UpdateStockAsync(int productId, int newStock, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeleteProductAsync(int id, CancellationToken cancellationToken = default);
        Task<ApiResponse<IEnumerable<ProductsResponseDto>>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default);
    }
}