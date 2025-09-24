using ProjectDemoWebApi.DTOs.Review;
using ProjectDemoWebApi.DTOs.Shared;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IReviewService
    {
        Task<ApiResponse<ReviewResponseDto>> CreateReviewAsync(string userId, CreateReviewDto createReviewDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<ReviewResponseDto>> UpdateReviewAsync(int reviewId, string userId, UpdateReviewDto updateReviewDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeleteReviewAsync(int reviewId, string userId, CancellationToken cancellationToken = default);
        Task<ApiResponse<ReviewResponseDto>> GetReviewByIdAsync(int reviewId, CancellationToken cancellationToken = default);
        Task<ApiResponse<IEnumerable<ReviewResponseDto>>> GetReviewsByProductIdAsync(int productId, CancellationToken cancellationToken = default);
        Task<ApiResponse<IEnumerable<ReviewResponseDto>>> GetReviewsByOrderIdAsync(int orderId, string userId, CancellationToken cancellationToken = default);
        Task<ApiResponse<IEnumerable<ReviewResponseDto>>> GetCustomerReviewsAsync(string userId, CancellationToken cancellationToken = default);

        // Admin functions
        Task<ApiResponse<IEnumerable<ReviewResponseDto>>> GetPendingReviewsAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> ApproveReviewAsync(int reviewId, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> RejectReviewAsync(int reviewId, string reason, CancellationToken cancellationToken = default);

        // Reply functions
        Task<ApiResponse<ReviewReplyDto>> AddReplyAsync(int reviewId, string userId, CreateReplyDto createReplyDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeleteReplyAsync(int replyId, string userId, CancellationToken cancellationToken = default);

        // Statistics
        Task<ApiResponse<object>> GetProductReviewStatsAsync(int productId, CancellationToken cancellationToken = default);
    }
}