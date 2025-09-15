using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.DTOs.Manufacturer;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace ProjectDemoWebApi.Services
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ManufacturerService> _logger;

        public ManufacturerService(IManufacturerRepository manufacturerRepository, IMapper mapper, ILogger<ManufacturerService> logger)
        {
            _manufacturerRepository = manufacturerRepository ?? throw new ArgumentNullException(nameof(manufacturerRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
        }

        public async Task<ApiResponse<IEnumerable<ManufacturerResponseDto>>> GetAllManufacturersAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var manufacturers = await _manufacturerRepository.GetAllAsync(cancellationToken);
                var manufacturerDtos = _mapper.Map<IEnumerable<ManufacturerResponseDto>>(manufacturers);
                
                return ApiResponse<IEnumerable<ManufacturerResponseDto>>.Ok(
                    manufacturerDtos, 
                    "Manufacturers retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ManufacturerResponseDto>>.Fail(
                    "An error occurred while retrieving manufacturers.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<PagedResponseDto<ManufacturerResponseDto>>> GetAllManufacturersPageAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
        {
            try
            {
                if (pageNumber <= 0) pageNumber = 1;
                if (pageSize <= 0) pageSize = 10;

                var (manufacturers, totalCount) = await _manufacturerRepository.GetPagedIncludeAsync(
                    pageNumber,
                    pageSize,
                    predicate: null,
                    cancellationToken
                );

                var manufacturerDtos = _mapper.Map<List<ManufacturerResponseDto>>(manufacturers);

                var response = new PagedResponseDto<ManufacturerResponseDto>
                {
                    Items = manufacturerDtos,
                    TotalCount = totalCount,
                    PageIndex = pageNumber,
                    PageSize = pageSize
                };

                return ApiResponse<PagedResponseDto<ManufacturerResponseDto>>.Ok(
                    response,
                    "Manufacturers retrieved successfully.",
                    200
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving manufacturers with pagination");
                return ApiResponse<PagedResponseDto<ManufacturerResponseDto>>.Fail(
                    "An error occurred while retrieving manufacturers.",
                    null,
                    500
                );
            }
        }

        public async Task<ApiResponse<PagedResponseDto<ManufacturerResponseDto>>> GetAllManufacturersPageAsync(
          int pageNumber = 1,
          int pageSize = 10,
          string? keyword = null,
          CancellationToken cancellationToken = default)
        {
            try
            {
                if (pageNumber <= 0) pageNumber = 1;
                if (pageSize <= 0) pageSize = 10;

                Expression<Func<Manufacturers, bool>>? predicate = null;
                if (!string.IsNullOrEmpty(keyword))
                {
                    predicate = m => m.ManufacturerName.Contains(keyword)
                                 || m.ManufacturerCode.Contains(keyword);
                }
                var (manufacturers, totalCount) = await _manufacturerRepository.GetPagedIncludeAsync(
                    pageNumber,
                    pageSize,
                    predicate,
                    cancellationToken,
                    includes: m => m.Products
                );
                var manufacturerDtos = manufacturers
                    .Select(m => new ManufacturerResponseDto
                    {
                        Id = m.Id,
                        ManufacturerCode = m.ManufacturerCode,
                        ManufacturerName = m.ManufacturerName,
                        IsActive = m.IsActive,
                        CreatedDate = m.CreatedDate,
                        ProductCount = m.Products.Count() 
                    })
                    .ToList();

                var response = new PagedResponseDto<ManufacturerResponseDto>
                {
                    Items = manufacturerDtos,
                    TotalCount = totalCount,
                    PageIndex = pageNumber,
                    PageSize = pageSize
                };

                return ApiResponse<PagedResponseDto<ManufacturerResponseDto>>.Ok(
                    response,
                    "Manufacturers retrieved successfully.",
                    200
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving manufacturers with pagination");
                return ApiResponse<PagedResponseDto<ManufacturerResponseDto>>.Fail(
                    "An error occurred while retrieving manufacturers.",
                    null,
                    500
                );
            }
        }



        public async Task<ApiResponse<IEnumerable<ManufacturerResponseDto>>> GetActiveManufacturersAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var manufacturers = await _manufacturerRepository.GetActiveManufacturersAsync(cancellationToken);
                var manufacturerDtos = _mapper.Map<IEnumerable<ManufacturerResponseDto>>(manufacturers);
                
                return ApiResponse<IEnumerable<ManufacturerResponseDto>>.Ok(
                    manufacturerDtos, 
                    "Active manufacturers retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ManufacturerResponseDto>>.Fail(
                    "An error occurred while retrieving active manufacturers.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<ManufacturerResponseDto?>> GetManufacturerByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return ApiResponse<ManufacturerResponseDto?>.Fail(
                        "Invalid manufacturer ID.", 
                        null, 
                        400
                    );
                }

                var manufacturer = await _manufacturerRepository.GetByIdAsync(id, cancellationToken);
                
                if (manufacturer == null)
                {
                    return ApiResponse<ManufacturerResponseDto?>.Fail(
                        "Manufacturer not found.", 
                        null, 
                        404
                    );
                }

                var manufacturerDto = _mapper.Map<ManufacturerResponseDto>(manufacturer);
                
                return ApiResponse<ManufacturerResponseDto?>.Ok(
                    manufacturerDto, 
                    "Manufacturer retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<ManufacturerResponseDto?>.Fail(
                    "An error occurred while retrieving the manufacturer.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<ManufacturerResponseDto?>> GetManufacturerByCodeAsync(string manufacturerCode, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(manufacturerCode))
                {
                    return ApiResponse<ManufacturerResponseDto?>.Fail(
                        "Manufacturer code cannot be empty.", 
                        null, 
                        400
                    );
                }

                var manufacturer = await _manufacturerRepository.GetByManufacturerCodeAsync(manufacturerCode, cancellationToken);
                
                if (manufacturer == null)
                {
                    return ApiResponse<ManufacturerResponseDto?>.Fail(
                        "Manufacturer not found.", 
                        null, 
                        404
                    );
                }

                var manufacturerDto = _mapper.Map<ManufacturerResponseDto>(manufacturer);
                
                return ApiResponse<ManufacturerResponseDto?>.Ok(
                    manufacturerDto, 
                    "Manufacturer retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<ManufacturerResponseDto?>.Fail(
                    "An error occurred while retrieving the manufacturer.", 
                    null, 
                    500
                );
            }
        }
        public async Task<ApiResponse<ManufacturerResponseDto>> CreateManufacturerAsync(CreateManufacturerDto createManufacturerDto, CancellationToken cancellationToken = default)
        {
            try
            {
                var prefix = createManufacturerDto.ManufacturerCode.ToUpper().Trim();

                if (prefix.Length != 3 || !Regex.IsMatch(prefix, @"^[A-Z]{3}$"))
                {
                    return ApiResponse<ManufacturerResponseDto>.Fail(
                        "Manufacturer code prefix must be exactly 3 uppercase letters (e.g., ABC).",
                        null,
                        400
                    );
                }

                var existingCodes = await _manufacturerRepository.GetCodesByPrefixAsync(prefix, cancellationToken);

                int nextNumber = 1;
                if (existingCodes.Any())
                {
                    var maxNum = existingCodes
                        .Select(code => int.TryParse(code.Substring(3), out var num) ? num : 0)
                        .Max();

                    nextNumber = maxNum + 1;
                }

                var finalCode = $"{prefix}{nextNumber:D2}";

                var codeExists = await _manufacturerRepository.IsManufacturerCodeExistsAsync(finalCode, null, cancellationToken);
                if (codeExists)
                {
                    return ApiResponse<ManufacturerResponseDto>.Fail(
                        "Manufacturer code already exists, please try again.",
                        null,
                        409
                    );
                }

                // Map sang entity
                var manufacturer = _mapper.Map<Manufacturers>(createManufacturerDto);
                manufacturer.ManufacturerCode = finalCode; // gán code auto-gen
                manufacturer.CreatedDate = DateTime.UtcNow;

                await _manufacturerRepository.AddAsync(manufacturer, cancellationToken);
                await _manufacturerRepository.SaveChangesAsync(cancellationToken);

                var manufacturerDto = _mapper.Map<ManufacturerResponseDto>(manufacturer);

                return ApiResponse<ManufacturerResponseDto>.Ok(
                    manufacturerDto,
                    "Manufacturer created successfully.",
                    201
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<ManufacturerResponseDto>.Fail(
                    "An error occurred while creating the manufacturer.",
                    null,
                    500
                );
            }
        }

        public async Task<ApiResponse<ManufacturerResponseDto?>> UpdateManufacturerAsync(int id, UpdateManufacturerDto updateManufacturerDto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return ApiResponse<ManufacturerResponseDto?>.Fail(
                        "Invalid manufacturer ID.", 
                        null, 
                        400
                    );
                }

                var manufacturer = await _manufacturerRepository.GetByIdAsync(id, cancellationToken);
                
                if (manufacturer == null)
                {
                    return ApiResponse<ManufacturerResponseDto?>.Fail(
                        "Manufacturer not found.", 
                        null, 
                        404
                    );
                }

                // Update only provided fields
                if (!string.IsNullOrWhiteSpace(updateManufacturerDto.ManufacturerName))
                    manufacturer.ManufacturerName = updateManufacturerDto.ManufacturerName;
                
                if (updateManufacturerDto.IsActive.HasValue)
                    manufacturer.IsActive = updateManufacturerDto.IsActive.Value;

                _manufacturerRepository.Update(manufacturer);
                await _manufacturerRepository.SaveChangesAsync(cancellationToken);

                var manufacturerDto = _mapper.Map<ManufacturerResponseDto>(manufacturer);
                
                return ApiResponse<ManufacturerResponseDto?>.Ok(
                    manufacturerDto, 
                    "Manufacturer updated successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<ManufacturerResponseDto?>.Fail(
                    "An error occurred while updating the manufacturer.", 
                    null, 
                    500
                );
            }
        }


public async Task<ApiResponse<bool>> DeleteManufacturerAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id <= 0)
            {
                return ApiResponse<bool>.Fail(
                    "Invalid manufacturer ID.",
                    false,
                    400
                );
            }

            var manufacturer = await _manufacturerRepository.GetByIdAsync(id, cancellationToken);

            if (manufacturer == null)
            {
                return ApiResponse<bool>.Fail(
                    "Manufacturer not found.",
                    false,
                    404
                );
            }

            _manufacturerRepository.Delete(manufacturer);
            await _manufacturerRepository.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(
                true,
                "Manufacturer deleted successfully.",
                200
            );
        }
        catch (DbUpdateException dbEx)
        {
            if (dbEx.InnerException is SqlException sqlEx && sqlEx.Number == 547) 
            {
                return ApiResponse<bool>.Fail(
                    "Cannot delete manufacturer because it is referenced by other data.",
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
                $"An error occurred while deleting the manufacturer: {ex.Message}",
                false,
                500
            );
        }
    }

    public async Task<ApiResponse<bool>> IsManufacturerCodeExistsAsync(string manufacturerCode, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(manufacturerCode))
                {
                    return ApiResponse<bool>.Fail(
                        "Manufacturer code cannot be empty.", 
                        false, 
                        400
                    );
                }

                var exists = await _manufacturerRepository.IsManufacturerCodeExistsAsync(manufacturerCode, excludeId, cancellationToken);
                
                return ApiResponse<bool>.Ok(
                    exists, 
                    "Manufacturer code existence checked successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(
                    "An error occurred while checking manufacturer code existence.", 
                    false, 
                    500
                );
            }
        }
    }
}