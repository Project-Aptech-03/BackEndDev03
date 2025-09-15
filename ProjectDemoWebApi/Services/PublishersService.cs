using AutoMapper;
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
    string? keyword = null,
    CancellationToken cancellationToken = default)
        {
            try
            {
                if (pageNumber <= 0) pageNumber = 1;
                if (pageSize <= 0) pageSize = 10;

                // predicate search
                Expression<Func<Publishers, bool>>? predicate = null;
                if (!string.IsNullOrEmpty(keyword))
                {
                    predicate = p => p.PublisherName.Contains(keyword);
                                  
                }

                (IEnumerable<Publishers> publishers, int totalCount) = await _publisherRepository.GetPagedIncludeAsync(
                    pageNumber,
                    pageSize,
                    predicate,
                    cancellationToken,
                    includes: p => p.Products
                );

                var publisherDtos = publishers
                    .Select(p => new PublisherResponseDto
                    {
                        Id = p.Id,
                        PublisherName = p.PublisherName,
                        PublisherAddress = p.PublisherAddress,
                        ContactInfo = p.ContactInfo,
                        IsActive = p.IsActive,
                        CreatedDate = p.CreatedDate,
                        ProductCount = p.Products.Count()
                    })
                    .ToList();

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



        public async Task<ApiResponse<PublisherResponseDto>> CreatePublisherAsync(CreatePublisherDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return ApiResponse<PublisherResponseDto>.Fail("Publisher data cannot be null.", null, 400);

                var publisher = _mapper.Map<Publishers>(dto);

                await _publisherRepository.AddAsync(publisher, cancellationToken);
                await _publisherRepository.SaveChangesAsync(cancellationToken);

                var responseDto = _mapper.Map<PublisherResponseDto>(publisher);

                return ApiResponse<PublisherResponseDto>.Ok(
                    responseDto,
                    "Publisher created successfully.",
                    201
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating publisher");
                return ApiResponse<PublisherResponseDto>.Fail(
                    "An error occurred while creating the publisher.",
                    null,
                    500
                );
            }
        }

            // ================== UPDATE ==================
            public async Task<ApiResponse<PublisherResponseDto>> UpdatePublisherAsync(int id, UpdatePublisherDto dto, CancellationToken cancellationToken = default)
            {
                try
                {
                    var entity = await _publisherRepository.GetByIdAsync(id, cancellationToken);
                    if (entity == null)
                        return ApiResponse<PublisherResponseDto>.Fail("Publisher not found", null, 404);

                    _mapper.Map(dto, entity);
                    _publisherRepository.Update(entity);
                    await _publisherRepository.SaveChangesAsync(cancellationToken);

                    var responseDto = _mapper.Map<PublisherResponseDto>(entity);
                    return ApiResponse<PublisherResponseDto>.Ok(responseDto, "Publisher updated successfully", 200);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating publisher {id}");
                    return ApiResponse<PublisherResponseDto>.Fail("An error occurred while updating the publisher", null, 500);
                }
            }

            // ================== DELETE ==================
            public async Task<ApiResponse<bool>> DeletePublisherAsync(int id, CancellationToken cancellationToken = default)
            {
                try
                {
                    var entity = await _publisherRepository.GetByIdAsync(id, cancellationToken);
                    if (entity == null)
                        return ApiResponse<bool>.Fail("Publisher not found", false, 404);

                    _publisherRepository.Delete(entity);
                    try
                    {
                        await _publisherRepository.SaveChangesAsync(cancellationToken);
                    }
                    catch (DbUpdateException dbEx) 
                    {
                        _logger.LogWarning(dbEx, "Cannot delete publisher due to existing references");
                        return ApiResponse<bool>.Fail(
                            "Cannot delete publisher because it has related products.",
                            false,
                            409
                        );
                    }

                    return ApiResponse<bool>.Ok(true, "Publisher deleted successfully", 200);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error deleting publisher {id}");
                    return ApiResponse<bool>.Fail("An error occurred while deleting the publisher", false, 500);
                }
            }

        }
        }

