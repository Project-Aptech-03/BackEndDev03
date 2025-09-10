using AutoMapper;
using ProjectDemoWebApi.DTOs.Manufacturer;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;

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
                var codeExists = await _manufacturerRepository.IsManufacturerCodeExistsAsync(createManufacturerDto.ManufacturerCode, null, cancellationToken);
                if (codeExists)
                {
                    return ApiResponse<ManufacturerResponseDto>.Fail(
                        "Manufacturer code already exists.", 
                        null, 
                        409
                    );
                }

                var manufacturer = _mapper.Map<Manufacturers>(createManufacturerDto);
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
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(
                    "An error occurred while deleting the manufacturer.", 
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