using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.DTOs.Category;
using ProjectDemoWebApi.DTOs.Publisher;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;
using System.Linq.Expressions;

namespace ProjectDemoWebApi.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
           
        }

        public async Task<ApiResponse<PagedResponseDto<CategoryResponseDto>>> GetAllCategoriesPageAsync(
     int pageNumber = 1,
     int pageSize = 10,
     string? keyword = null,
     CancellationToken cancellationToken = default)
        {
            try
            {
                if (pageNumber <= 0) pageNumber = 1;
                if (pageSize <= 0) pageSize = 10;

                Expression<Func<Categories, bool>>? predicate = null;
                if (!string.IsNullOrEmpty(keyword))
                {
                    predicate = c => c.CategoryName.Contains(keyword)
                                 || c.CategoryCode.Contains(keyword);
                }

                var (categories, totalCount) = await _categoryRepository.GetPagedIncludeAsync(
                    pageNumber,
                    pageSize,
                    predicate,
                    cancellationToken,
                    includes: c => c.Products
                );

                var categoryDtos = categories
                    .Select(c => new CategoryResponseDto
                    {
                        Id = c.Id,
                        CategoryCode = c.CategoryCode,
                        CategoryName = c.CategoryName,
                        IsActive = c.IsActive,
                        CreatedDate = c.CreatedDate,
                        ProductCount = c.Products.Count()
                    })
                    .ToList();

                var response = new PagedResponseDto<CategoryResponseDto>
                {
                    Items = categoryDtos,
                    TotalCount = totalCount,
                    PageIndex = pageNumber,
                    PageSize = pageSize,
                };

                return ApiResponse<PagedResponseDto<CategoryResponseDto>>.Ok(
                    response,
                    "Categories retrieved successfully.",
                    200
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving categories with pagination");
                return ApiResponse<PagedResponseDto<CategoryResponseDto>>.Fail(
                    "An error occurred while retrieving categories.",
                    null,
                    500
                );
            }
        }





        public async Task<ApiResponse<PagedResponseDto<CategoryResponseDto>>> GetActiveCategoriesPagedAsync(
    int pageNumber = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default)
        {
            try
            {
                if (pageNumber <= 0) pageNumber = 1;
                if (pageSize <= 0) pageSize = 10;

                var (categories, totalCount) = await _categoryRepository.GetActiveCategoriesPagedAsync(
                    pageNumber, pageSize, cancellationToken);

                var categoryDtos = _mapper.Map<List<CategoryResponseDto>>(categories);

                var response = new PagedResponseDto<CategoryResponseDto>
                {
                    Items = categoryDtos,
                    TotalCount = totalCount,
                    PageIndex = pageNumber,
                    PageSize = pageSize
                };

                return ApiResponse<PagedResponseDto<CategoryResponseDto>>.Ok(
                    response,
                    "Active categories retrieved successfully.",
                    200
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active categories with pagination");
                return ApiResponse<PagedResponseDto<CategoryResponseDto>>.Fail(
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
                var prefix = createCategoryDto.CategoryCode.ToUpper();

                if (prefix.Length != 1 || !char.IsLetter(prefix[0]))
                {
                    return ApiResponse<CategoryResponseDto>.Fail(
                        "Category code must be exactly 1 letter.",
                        null,
                        400
                    );
                }

                var existingCategories = await _categoryRepository.GetCategoriesByPrefixAsync(prefix, cancellationToken);

                int maxNumber = 0;
                foreach (var cat in existingCategories)
                {
                    var numberPart = cat.CategoryCode.Substring(1); 
                    if (int.TryParse(numberPart, out var n))
                    {
                        if (n > maxNumber) maxNumber = n;
                    }
                }

                int nextNumber = maxNumber + 1;

                var newCode = $"{prefix}{nextNumber}";

                var codeExists = await _categoryRepository.IsCategoryCodeExistsAsync(newCode, null, cancellationToken);
                if (codeExists)
                {
                    return ApiResponse<CategoryResponseDto>.Fail(
                        "Category code already exists.",
                        null,
                        409
                    );
                }

                createCategoryDto.CategoryCode = newCode;

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
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException is SqlException sqlEx && sqlEx.Number == 547) 
                {
                    return ApiResponse<bool>.Fail(
                        "Cannot delete category because it is referenced by other data.",
                        false,
                        400
                    );
                }

                return ApiResponse<bool>.Fail(
                    $"Database error: {dbEx.Message}",
                    false,
                    500
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(
                    $"An error occurred while deleting the category: {ex.Message}",
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