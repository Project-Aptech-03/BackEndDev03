using AutoMapper;
using ProjectDemoWebApi.DTOs.Category;
using ProjectDemoWebApi.DTOs.Publisher;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Services
{
    public class PublishersService : IPublishersService
    {
        readonly IPublisherRepository _publisherRepository;
        readonly IMapper _mapper;
        private readonly ILogger<PublishersService> _logger;

        public PublishersService(IPublisherRepository publisherRepository, IMapper mapper, ILogger<PublishersService> logger)
        {
            _publisherRepository = publisherRepository ?? throw new ArgumentNullException(nameof(publisherRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
        }

        //public Task<ApiResponse<PagedResponseDto<PublisherResponseDto>>> GetAllPublisher ()

        public async Task<ApiResponse<IEnumerable<PublisherResponseDto>>> GetAllPublishersAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var categories = await _publisherRepository.GetAllAsync(cancellationToken);
                var categoryDtos = _mapper.Map<IEnumerable<PublisherResponseDto>>(categories);

                return ApiResponse<IEnumerable<PublisherResponseDto>>.Ok(
                    categoryDtos,
                    "Publishers retrieved successfully.",
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<PublisherResponseDto>>.Fail(
                    "An error occurred while retrieving publishers.",
                    null,
                    500
                );
            }
        }

        public async Task<ApiResponse<PagedResponseDto<PublisherResponseDto>>> GetAllPublishersPageAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
        {
            try
            {
                if (pageNumber <= 0) pageNumber = 1;
                if (pageSize <= 0) pageSize = 10;

                var (publishers, totalCount) = await _publisherRepository.GetPagedIncludeAsync(
                    pageNumber,
                    pageSize,
                    predicate: null,
                    cancellationToken
                );

                var publisherDtos = _mapper.Map<List<PublisherResponseDto>>(publishers);

                var response = new PagedResponseDto<PublisherResponseDto>
                {
                    Items = publisherDtos,
                    TotalCount = totalCount,
                    PageIndex = pageNumber,
                    PageSize = pageSize
                };

                return ApiResponse<PagedResponseDto<PublisherResponseDto>>.Ok(
                    response,
                    "Publishers retrieved successfully.",
                    200
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving publishers with pagination");
                return ApiResponse<PagedResponseDto<PublisherResponseDto>>.Fail(
                    "An error occurred while retrieving publishers.",
                    null,
                    500
                );
            }
        }

    }
}
