using ProjectDemoWebApi.DTOs.Publisher;
using ProjectDemoWebApi.DTOs.Shared;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IPublishersService

    {
        Task<ApiResponse<PagedResponseDto<PublisherResponseDto>>> GetAllPublishersPageAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<PublisherResponseDto>>> GetAllPublishersAsync(CancellationToken cancellationToken = default);
    }
}
