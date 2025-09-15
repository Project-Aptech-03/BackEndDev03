using ProjectDemoWebApi.DTOs.Category;
using ProjectDemoWebApi.DTOs.Shared;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface ICategoryService
    {   
        Task<ApiResponse<PagedResponseDto<CategoryResponseDto>>> GetAllCategoriesPageAsync(
         int pageNumber = 1,
         int pageSize = 10,
         CancellationToken cancellationToken = default);
        Task<ApiResponse<PagedResponseDto<CategoryResponseDto>>> GetActiveCategoriesPagedAsync(
       int pageNumber = 1,
       int pageSize = 10,
       CancellationToken cancellationToken = default);
        Task<ApiResponse<CategoryResponseDto?>> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ApiResponse<CategoryResponseDto?>> GetCategoryByCodeAsync(string categoryCode, CancellationToken cancellationToken = default);
        Task<ApiResponse<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryDto createCategoryDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<CategoryResponseDto?>> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> IsCategoryCodeExistsAsync(string categoryCode, int? excludeId = null, CancellationToken cancellationToken = default);
    }
}