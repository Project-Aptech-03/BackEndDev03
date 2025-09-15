using ProjectDemoWebApi.DTOs.Manufacturer;
using ProjectDemoWebApi.DTOs.Shared;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IManufacturerService
    {
        Task<ApiResponse<IEnumerable<ManufacturerResponseDto>>> GetAllManufacturersAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<PagedResponseDto<ManufacturerResponseDto>>> GetAllManufacturersPageAsync(
          int pageNumber = 1,
          int pageSize = 10,
          string? keyword = null,
          CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<ManufacturerResponseDto>>> GetActiveManufacturersAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<ManufacturerResponseDto?>> GetManufacturerByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ApiResponse<ManufacturerResponseDto?>> GetManufacturerByCodeAsync(string manufacturerCode, CancellationToken cancellationToken = default);
        Task<ApiResponse<ManufacturerResponseDto>> CreateManufacturerAsync(CreateManufacturerDto createManufacturerDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<ManufacturerResponseDto?>> UpdateManufacturerAsync(int id, UpdateManufacturerDto updateManufacturerDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeleteManufacturerAsync(int id, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> IsManufacturerCodeExistsAsync(string manufacturerCode, int? excludeId = null, CancellationToken cancellationToken = default);
    }
}