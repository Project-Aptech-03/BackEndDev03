using ProjectDemoWebApi.DTOs.Publisher;
using ProjectDemoWebApi.DTOs.Shared;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IPublishersService

    {
        Task<ApiResponse<PagedResponseDto<PublisherResponseDto>>> GetAllPublishersPageAsync(
    int pageNumber = 1,
    int pageSize = 10,
    string? keyword = null,
    CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<PublisherResponseDto>>> GetAllPublishersAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<PublisherResponseDto>> CreatePublisherAsync(CreatePublisherDto dto, CancellationToken cancellationToken = default);
        Task<ApiResponse<PublisherResponseDto>> UpdatePublisherAsync(int id, UpdatePublisherDto dto, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeletePublisherAsync(int id, CancellationToken cancellationToken = default);
    }
}
