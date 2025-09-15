using ProjectDemoWebApi.DTOs.Products;
using ProjectDemoWebApi.DTOs.Shared;
using System.Threading.Tasks;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IProductsService
    {
        Task<ApiResponse<PagedResponseDto<ProductsResponseDto>>> GetProductsPagedAsync(
        int pageNumber,
        int pageSize,
        string? keyword = null,
        int? categoriesId = null,
        int? manufacturerId = null,
        CancellationToken cancellationToken = default);
        //Task<ApiResponse<PagedResponseDto<ProductsResponseDto>>> GetProductsPagedAsync(
        //int pageNumber,
        //int pageSize,
        //string? keyword = null,
        //CancellationToken cancellationToken = default);

        Task<ApiResponse<ProductsResponseDto>> CreateProductAsync(CreateProductsDto createProductDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<ProductsResponseDto?>> UpdateProductAsync(int id, UpdateProductsDto updateProductDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeleteProductAsync(int id, CancellationToken cancellationToken = default);
        Task<ApiResponse<int>> DeleteProductsAsync(List<int> ids, CancellationToken cancellationToken = default);

         Task<string> GenerateProductCodeAsync(
         int categoryId,
         int manufacturerId,
         CancellationToken cancellationToken);

        Task<ApiResponse<IEnumerable<ProductsResponseDto>>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<ProductsResponseDto?>> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ApiResponse<ProductsResponseDto?>> GetProductByCodeAsync(string productCode, CancellationToken cancellationToken = default);
        Task<ApiResponse<IEnumerable<ProductsResponseDto>>> GetProductsByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<ApiResponse<IEnumerable<ProductsResponseDto>>> GetProductsByManufacturerAsync(int manufacturerId, CancellationToken cancellationToken = default);
        Task<ApiResponse<IEnumerable<ProductsResponseDto>>> GetProductsByPublisherAsync(int publisherId, CancellationToken cancellationToken = default);
        Task<ApiResponse<IEnumerable<ProductsResponseDto>>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<ApiResponse<IEnumerable<ProductsResponseDto>>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> UpdateStockAsync(int productId, int newStock, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> IsProductCodeExistsAsync(string productCode, int? excludeId = null, CancellationToken cancellationToken = default);
    }
}