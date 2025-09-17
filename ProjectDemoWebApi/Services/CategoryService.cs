using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.DTOs.Category;
using ProjectDemoWebApi.DTOs.Publisher;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.DTOs.SubCategory;
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
        private readonly ISubCategoryRepository _subCategoryRepository;
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, ILogger<CategoryService> logger, ISubCategoryRepository subCategoryRepository)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
            _subCategoryRepository = subCategoryRepository;
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
                    c => c.Products,
                    c => c.SubCategories 
                );

                var categoryDtos = categories
                .Select(c => new CategoryResponseDto
                {
                    Id = c.Id,
                    CategoryCode = c.CategoryCode,
                    CategoryName = c.CategoryName,
                    IsActive = c.IsActive,
                    CreatedDate = c.CreatedDate,
                    ProductCount = c.Products.Count(),
                    SubCategories = c.SubCategories?.Select(sc => new SubCategoryResponseDto
                    {
                        Id = sc.Id,
                        SubCategoryName = sc.SubCategoryName,
                        IsActive = sc.IsActive,
                        CreatedDate = sc.CreatedDate
                    }).ToList()
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
                    if (cat.CategoryCode.Length > 1)
                    {
                        var numberPart = cat.CategoryCode.Substring(1);
                        if (int.TryParse(numberPart, out var n))
                        {
                            if (n > maxNumber) maxNumber = n;
                        }
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

                var category = new Categories
                {
                    CategoryCode = newCode,
                    CategoryName = createCategoryDto.CategoryName,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    SubCategories = new List<SubCategories>()
                };

                await _categoryRepository.AddAsync(category, cancellationToken);
                await _categoryRepository.SaveChangesAsync(cancellationToken); 

                if (createCategoryDto.SubCategories != null && createCategoryDto.SubCategories.Any())
                {
                    int counter = 1;
                    foreach (var subDto in createCategoryDto.SubCategories)
                    {
                        if (string.IsNullOrWhiteSpace(subDto.SubCategoryName)) continue;

                        var subCategory = new SubCategories
                        {
                            SubCategoryCode = $"{newCode}S{counter:D2}",
                            SubCategoryName = subDto.SubCategoryName.Trim(),
                            IsActive = true,
                            CreatedDate = DateTime.UtcNow,
                            CategoryId = category.Id
                        };

                        await _subCategoryRepository.AddAsync(subCategory, cancellationToken);
                        counter++;
                    }

                    await _subCategoryRepository.SaveChangesAsync(cancellationToken);
                }

                var createdCategory = await _categoryRepository.GetByIdWithSubCategoriesAsync(category.Id, cancellationToken);
                var categoryDto = _mapper.Map<CategoryResponseDto>(createdCategory);

                return ApiResponse<CategoryResponseDto>.Ok(
                    categoryDto,
                    "Category created successfully.",
                    201
                );
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? "No inner exception";
                var fullMessage = $"Main: {ex.Message} | Inner: {innerMessage}";

                return ApiResponse<CategoryResponseDto>.Fail(
                    $"An error occurred while creating the category: {fullMessage}",
                    null,
                    500
                );
            }
        }



        public async Task<ApiResponse<CategoryResponseDto?>> UpdateCategoryAsync(
      int id,
      UpdateCategoryDto updateCategoryDto,
      CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return ApiResponse<CategoryResponseDto?>.Fail("Invalid category ID.", null, 400);

                var category = await _categoryRepository.GetByIdWithSubCategoriesAsync(id, cancellationToken);
                if (category == null)
                    return ApiResponse<CategoryResponseDto?>.Fail("Category not found.", null, 404);

                if (!string.IsNullOrWhiteSpace(updateCategoryDto.CategoryName))
                    category.CategoryName = updateCategoryDto.CategoryName;

                if (updateCategoryDto.IsActive.HasValue)
                    category.IsActive = updateCategoryDto.IsActive.Value;

                if (updateCategoryDto.SubCategories != null)
                {
                    var incomingSubs = updateCategoryDto.SubCategories;
                    var incomingIds = incomingSubs.Where(s => s.Id > 0).Select(s => s.Id).ToList();

                    var existingSubs = category.SubCategories?.ToList() ?? new List<SubCategories>();

                    foreach (var existing in existingSubs)
                    {
                        if (!incomingIds.Contains(existing.Id))
                        {
                            category.SubCategories.Remove(existing); 
                        }
                    }

                    foreach (var sub in incomingSubs)
                    {
                        if (sub.Id > 0)
                        {
                            var existing = existingSubs.FirstOrDefault(s => s.Id == sub.Id);
                            if (existing != null)
                            {
                                existing.SubCategoryName = sub.SubCategoryName;
                            }
                        }
                        else
                        {
                            var newSub = new SubCategories
                            {
                                SubCategoryName = sub.SubCategoryName.Trim(),
                                SubCategoryCode = $"{category.CategoryCode}{Guid.NewGuid().ToString()[..1].ToUpper()}",
                                IsActive = true,
                                CreatedDate = DateTime.UtcNow,
                                CategoryId = category.Id
                            };
                            category.SubCategories.Add(newSub);
                        }
                    }
                }

                _categoryRepository.Update(category);
                await _categoryRepository.SaveChangesAsync(cancellationToken);

                var updatedCategory = await _categoryRepository.GetByIdWithSubCategoriesAsync(id, cancellationToken);

                var categoryDto = _mapper.Map<CategoryResponseDto>(updatedCategory);

                return ApiResponse<CategoryResponseDto?>.Ok(
                    categoryDto,
                    "Category updated successfully.",
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<CategoryResponseDto?>.Fail(
                    $"An error occurred while updating the category: {ex.Message}",
                    null,
                    500
                );
            }
        }

        public async Task<ApiResponse<bool>> DeleteCategoryAsync(
     int id,
     CancellationToken cancellationToken = default)
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