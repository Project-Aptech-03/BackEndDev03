using AutoMapper;
using ProjectDemoWebApi.DTOs.Category;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ApiResponse<IEnumerable<CategoryResponseDto>>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync(cancellationToken);
                var categoryDtos = _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
                
                return ApiResponse<IEnumerable<CategoryResponseDto>>.Ok(
                    categoryDtos, 
                    "Categories retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CategoryResponseDto>>.Fail(
                    "An error occurred while retrieving categories.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<CategoryResponseDto>>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var categories = await _categoryRepository.GetActiveCategoriesAsync(cancellationToken);
                var categoryDtos = _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
                
                return ApiResponse<IEnumerable<CategoryResponseDto>>.Ok(
                    categoryDtos, 
                    "Active categories retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CategoryResponseDto>>.Fail(
                    "An error occurred while retrieving active categories.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<CategoryResponseDto?>> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return ApiResponse<CategoryResponseDto?>.Fail(
                        "Invalid category ID.", 
                        null, 
                        400
                    );
                }

                var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
                
                if (category == null)
                {
                    return ApiResponse<CategoryResponseDto?>.Fail(
                        "Category not found.", 
                        null, 
                        404
                    );
                }

                var categoryDto = _mapper.Map<CategoryResponseDto>(category);
                
                return ApiResponse<CategoryResponseDto?>.Ok(
                    categoryDto, 
                    "Category retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<CategoryResponseDto?>.Fail(
                    "An error occurred while retrieving the category.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<CategoryResponseDto?>> GetCategoryByCodeAsync(string categoryCode, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(categoryCode))
                {
                    return ApiResponse<CategoryResponseDto?>.Fail(
                        "Category code cannot be empty.", 
                        null, 
                        400
                    );
                }

                var category = await _categoryRepository.GetByCategoryCodeAsync(categoryCode, cancellationToken);
                
                if (category == null)
                {
                    return ApiResponse<CategoryResponseDto?>.Fail(
                        "Category not found.", 
                        null, 
                        404
                    );
                }

                var categoryDto = _mapper.Map<CategoryResponseDto>(category);
                
                return ApiResponse<CategoryResponseDto?>.Ok(
                    categoryDto, 
                    "Category retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<CategoryResponseDto?>.Fail(
                    "An error occurred while retrieving the category.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryDto createCategoryDto, CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if category code already exists
                var codeExists = await _categoryRepository.IsCategoryCodeExistsAsync(createCategoryDto.CategoryCode, null, cancellationToken);
                if (codeExists)
                {
                    return ApiResponse<CategoryResponseDto>.Fail(
                        "Category code already exists.", 
                        null, 
                        409
                    );
                }

                var category = _mapper.Map<Categories>(createCategoryDto);
                category.CreatedDate = DateTime.UtcNow;

                await _categoryRepository.AddAsync(category, cancellationToken);
                await _categoryRepository.SaveChangesAsync(cancellationToken);

                var categoryDto = _mapper.Map<CategoryResponseDto>(category);
                
                return ApiResponse<CategoryResponseDto>.Ok(
                    categoryDto, 
                    "Category created successfully.", 
                    201
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<CategoryResponseDto>.Fail(
                    "An error occurred while creating the category.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<CategoryResponseDto?>> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return ApiResponse<CategoryResponseDto?>.Fail(
                        "Invalid category ID.", 
                        null, 
                        400
                    );
                }

                var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
                
                if (category == null)
                {
                    return ApiResponse<CategoryResponseDto?>.Fail(
                        "Category not found.", 
                        null, 
                        404
                    );
                }

                // Update only provided fields
                if (!string.IsNullOrWhiteSpace(updateCategoryDto.CategoryName))
                    category.CategoryName = updateCategoryDto.CategoryName;
                
                if (updateCategoryDto.IsActive.HasValue)
                    category.IsActive = updateCategoryDto.IsActive.Value;

                _categoryRepository.Update(category);
                await _categoryRepository.SaveChangesAsync(cancellationToken);

                var categoryDto = _mapper.Map<CategoryResponseDto>(category);
                
                return ApiResponse<CategoryResponseDto?>.Ok(
                    categoryDto, 
                    "Category updated successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<CategoryResponseDto?>.Fail(
                    "An error occurred while updating the category.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<bool>> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return ApiResponse<bool>.Fail(
                        "Invalid category ID.", 
                        false, 
                        400
                    );
                }

                var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
                
                if (category == null)
                {
                    return ApiResponse<bool>.Fail(
                        "Category not found.", 
                        false, 
                        404
                    );
                }

                _categoryRepository.Delete(category);
                await _categoryRepository.SaveChangesAsync(cancellationToken);
                
                return ApiResponse<bool>.Ok(
                    true, 
                    "Category deleted successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(
                    "An error occurred while deleting the category.", 
                    false, 
                    500
                );
            }
        }

        public async Task<ApiResponse<bool>> IsCategoryCodeExistsAsync(string categoryCode, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(categoryCode))
                {
                    return ApiResponse<bool>.Fail(
                        "Category code cannot be empty.", 
                        false, 
                        400
                    );
                }

                var exists = await _categoryRepository.IsCategoryCodeExistsAsync(categoryCode, excludeId, cancellationToken);
                
                return ApiResponse<bool>.Ok(
                    exists, 
                    "Category code existence checked successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(
                    "An error occurred while checking category code existence.", 
                    false, 
                    500
                );
            }
        }
    }
}