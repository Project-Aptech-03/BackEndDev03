using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.DTOs.Review;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;
using System.Security.Claims;

namespace ProjectDemoWebApi.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IProductReviewRepository _reviewRepository;
        private readonly IReviewImageRepository _reviewImageRepository;
        private readonly IReviewReplyRepository _reviewReplyRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductsRepository _productsRepository;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public ReviewService(
            IProductReviewRepository reviewRepository,
            IReviewImageRepository reviewImageRepository,
            IReviewReplyRepository reviewReplyRepository,
            IOrderRepository orderRepository,
            IProductsRepository productsRepository,
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment,
            IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _reviewImageRepository = reviewImageRepository;
            _reviewReplyRepository = reviewReplyRepository;
            _orderRepository = orderRepository;
            _productsRepository = productsRepository;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ReviewResponseDto>> CreateReviewAsync(string userId, CreateReviewDto createReviewDto, CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate order and product
                var validationResult = await ValidateReviewCreationAsync(userId, createReviewDto, cancellationToken);
                if (!validationResult.Success)
                    return validationResult;

                // Check if customer already reviewed this product from this order
                var hasReviewed = await _reviewRepository.HasCustomerReviewedProductAsync(
                    userId, createReviewDto.ProductId, createReviewDto.OrderId, cancellationToken);

                if (hasReviewed)
                    return ApiResponse<ReviewResponseDto>.Fail("You have already reviewed this product from this order.", null, 400);

                // Create review
                var review = new ProductReviews
                {
                    OrderId = createReviewDto.OrderId,
                    ProductId = createReviewDto.ProductId,
                    CustomerId = userId,
                    Rating = createReviewDto.Rating,
                    Comment = createReviewDto.Comment,
                    ReviewDate = DateTime.UtcNow,
                    IsApproved = true, // Require admin approval
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                await _reviewRepository.AddAsync(review, cancellationToken);
                await _reviewRepository.SaveChangesAsync(cancellationToken);

                // Save review images
                if (createReviewDto.ReviewImages != null && createReviewDto.ReviewImages.Any())
                {
                    await SaveReviewImagesAsync(review.Id, createReviewDto.ReviewImages, cancellationToken);
                }

                // Get the complete review with details
                var createdReview = await _reviewRepository.GetReviewWithDetailsAsync(review.Id, cancellationToken);
                var reviewDto = _mapper.Map<ReviewResponseDto>(createdReview);

                return ApiResponse<ReviewResponseDto>.Ok(reviewDto, "Review submitted successfully and is now visible.", 201);
            }
            catch (Exception ex)
            {
                return ApiResponse<ReviewResponseDto>.Fail($"An error occurred while creating review: {ex.Message}", null, 500);
            }
        }

        public async Task<ApiResponse<ReviewResponseDto>> UpdateReviewAsync(int reviewId, string userId, UpdateReviewDto updateReviewDto, CancellationToken cancellationToken = default)
        {
            try
            {
                var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
                if (review == null || !review.IsActive)
                    return ApiResponse<ReviewResponseDto>.Fail("Review not found.", null, 404);

                if (review.CustomerId != userId)
                    return ApiResponse<ReviewResponseDto>.Fail("You can only update your own reviews.", null, 403);

                // Update fields
                if (updateReviewDto.Rating.HasValue)
                    review.Rating = updateReviewDto.Rating.Value;

                if (!string.IsNullOrWhiteSpace(updateReviewDto.Comment))
                    review.Comment = updateReviewDto.Comment;

                review.UpdatedDate = DateTime.UtcNow;

                // Handle image updates
                if (updateReviewDto.ImagesToDelete != null && updateReviewDto.ImagesToDelete.Any())
                {
                    await DeleteReviewImagesAsync(updateReviewDto.ImagesToDelete, cancellationToken);
                }

                if (updateReviewDto.NewReviewImages != null && updateReviewDto.NewReviewImages.Any())
                {
                    await SaveReviewImagesAsync(reviewId, updateReviewDto.NewReviewImages, cancellationToken);
                }

                _reviewRepository.Update(review);
                await _reviewRepository.SaveChangesAsync(cancellationToken);

                var updatedReview = await _reviewRepository.GetReviewWithDetailsAsync(reviewId, cancellationToken);
                var reviewDto = _mapper.Map<ReviewResponseDto>(updatedReview);

                return ApiResponse<ReviewResponseDto>.Ok(reviewDto, "Review updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<ReviewResponseDto>.Fail($"An error occurred while updating review: {ex.Message}", null, 500);
            }
        }

        public async Task<ApiResponse<bool>> DeleteReviewAsync(int reviewId, string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
                if (review == null || !review.IsActive)
                    return ApiResponse<bool>.Fail("Review not found.", false, 404);

                if (review.CustomerId != userId)
                    return ApiResponse<bool>.Fail("You can only delete your own reviews.", false, 403);

                review.IsActive = false;
                review.UpdatedDate = DateTime.UtcNow;

                _reviewRepository.Update(review);
                await _reviewRepository.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Ok(true, "Review deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail($"An error occurred while deleting review: {ex.Message}", false, 500);
            }
        }

        public async Task<ApiResponse<ReviewResponseDto>> GetReviewByIdAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            try
            {
                var review = await _reviewRepository.GetReviewWithDetailsAsync(reviewId, cancellationToken);
                if (review == null || !review.IsActive)
                    return ApiResponse<ReviewResponseDto>.Fail("Review not found.", null, 404);

                var reviewDto = _mapper.Map<ReviewResponseDto>(review);
                return ApiResponse<ReviewResponseDto>.Ok(reviewDto, "Review retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<ReviewResponseDto>.Fail($"An error occurred while retrieving review: {ex.Message}", null, 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<ReviewResponseDto>>> GetReviewsByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            try
            {
                var reviews = await _reviewRepository.GetReviewsByProductIdAsync(productId, cancellationToken);
                var reviewDtos = _mapper.Map<IEnumerable<ReviewResponseDto>>(reviews);

                return ApiResponse<IEnumerable<ReviewResponseDto>>.Ok(reviewDtos, "Product reviews retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ReviewResponseDto>>.Fail($"An error occurred while retrieving product reviews: {ex.Message}", null, 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<ReviewResponseDto>>> GetReviewsByOrderIdAsync(int orderId, string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                // Verify user owns the order
                var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
                if (order == null || order.CustomerId != userId)
                    return ApiResponse<IEnumerable<ReviewResponseDto>>.Fail("Order not found or access denied.", null, 404);

                var reviews = await _reviewRepository.GetReviewsByOrderIdAsync(orderId, cancellationToken);
                var reviewDtos = _mapper.Map<IEnumerable<ReviewResponseDto>>(reviews);

                return ApiResponse<IEnumerable<ReviewResponseDto>>.Ok(reviewDtos, "Order reviews retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ReviewResponseDto>>.Fail($"An error occurred while retrieving order reviews: {ex.Message}", null, 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<ReviewResponseDto>>> GetCustomerReviewsAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var reviews = await _reviewRepository.GetReviewsByCustomerIdAsync(userId, cancellationToken);
                var reviewDtos = _mapper.Map<IEnumerable<ReviewResponseDto>>(reviews);

                return ApiResponse<IEnumerable<ReviewResponseDto>>.Ok(reviewDtos, "Customer reviews retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ReviewResponseDto>>.Fail($"An error occurred while retrieving customer reviews: {ex.Message}", null, 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<ReviewResponseDto>>> GetPendingReviewsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var reviews = await _reviewRepository.GetPendingReviewsAsync(cancellationToken);
                var reviewDtos = _mapper.Map<IEnumerable<ReviewResponseDto>>(reviews);

                return ApiResponse<IEnumerable<ReviewResponseDto>>.Ok(reviewDtos, "Pending reviews retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ReviewResponseDto>>.Fail($"An error occurred while retrieving pending reviews: {ex.Message}", null, 500);
            }
        }

        public async Task<ApiResponse<bool>> ApproveReviewAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            try
            {
                var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
                if (review == null || !review.IsActive)
                    return ApiResponse<bool>.Fail("Review not found.", false, 404);

                review.IsApproved = true;
                review.UpdatedDate = DateTime.UtcNow;

                _reviewRepository.Update(review);
                await _reviewRepository.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Ok(true, "Review approved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail($"An error occurred while approving review: {ex.Message}", false, 500);
            }
        }

        public async Task<ApiResponse<bool>> RejectReviewAsync(int reviewId, string reason, CancellationToken cancellationToken = default)
        {
            try
            {
                var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
                if (review == null || !review.IsActive)
                    return ApiResponse<bool>.Fail("Review not found.", false, 404);

                review.IsActive = false;
                review.Comment = $"[REJECTED: {reason}] " + review.Comment;
                review.UpdatedDate = DateTime.UtcNow;

                _reviewRepository.Update(review);
                await _reviewRepository.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Ok(true, "Review rejected successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail($"An error occurred while rejecting review: {ex.Message}", false, 500);
            }
        }

        public async Task<ApiResponse<ReviewReplyDto>> AddReplyAsync(int reviewId, string userId, CreateReplyDto createReplyDto, CancellationToken cancellationToken = default)
        {
            try
            {
                var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
                if (review == null || !review.IsActive)
                    return ApiResponse<ReviewReplyDto>.Fail("Review not found.", null, 404);

                // Check if parent reply exists if provided
                if (createReplyDto.ParentReplyId.HasValue)
                {
                    var parentReply = await _reviewReplyRepository.GetByIdAsync(createReplyDto.ParentReplyId.Value, cancellationToken);
                    if (parentReply == null || !parentReply.IsActive || parentReply.ReviewId != reviewId)
                        return ApiResponse<ReviewReplyDto>.Fail("Invalid parent reply.", null, 400);
                }

                // Sửa lỗi: Kiểm tra quyền admin bằng cách truy vấn trực tiếp bảng Roles
                var isAdmin = await _context.UserRoles
                    .Join(_context.Roles,
                        ur => ur.RoleId,
                        r => r.Id,
                        (ur, r) => new { UserRole = ur, Role = r })
                    .AnyAsync(x => x.UserRole.UserId == userId && x.Role.Name == "Admin", cancellationToken);

                var reply = new ReviewReplies
                {
                    ReviewId = reviewId,
                    ParentReplyId = createReplyDto.ParentReplyId,
                    UserId = userId,
                    IsAdminReply = isAdmin,
                    Comment = createReplyDto.Comment,
                    ReplyDate = DateTime.UtcNow,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };

                await _reviewReplyRepository.AddAsync(reply, cancellationToken);
                await _reviewReplyRepository.SaveChangesAsync(cancellationToken);

                var replyWithDetails = await _reviewReplyRepository.GetReplyWithDetailsAsync(reply.Id, cancellationToken);
                var replyDto = _mapper.Map<ReviewReplyDto>(replyWithDetails);

                return ApiResponse<ReviewReplyDto>.Ok(replyDto, "Reply added successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<ReviewReplyDto>.Fail($"An error occurred while adding reply: {ex.Message}", null, 500);
            }
        }

        public async Task<ApiResponse<bool>> DeleteReplyAsync(int replyId, string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var reply = await _reviewReplyRepository.GetByIdAsync(replyId, cancellationToken);
                if (reply == null || !reply.IsActive)
                    return ApiResponse<bool>.Fail("Reply not found.", false, 404);

                // Sửa lỗi: Kiểm tra quyền admin bằng cách truy vấn trực tiếp bảng Roles
                var isAdmin = await _context.UserRoles
                    .Join(_context.Roles,
                        ur => ur.RoleId,
                        r => r.Id,
                        (ur, r) => new { UserRole = ur, Role = r })
                    .AnyAsync(x => x.UserRole.UserId == userId && x.Role.Name == "Admin", cancellationToken);

                // Check if user owns the reply or is admin
                if (reply.UserId != userId && !isAdmin)
                    return ApiResponse<bool>.Fail("You can only delete your own replies.", false, 403);

                reply.IsActive = false;

                _reviewReplyRepository.Update(reply);
                await _reviewReplyRepository.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Ok(true, "Reply deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail($"An error occurred while deleting reply: {ex.Message}", false, 500);
            }
        }

        public async Task<ApiResponse<object>> GetProductReviewStatsAsync(int productId, CancellationToken cancellationToken = default)
        {
            try
            {
                var averageRating = await _reviewRepository.GetAverageRatingByProductIdAsync(productId, cancellationToken);
                var reviewCount = await _reviewRepository.GetReviewCountByProductIdAsync(productId, cancellationToken);

                // Get rating distribution
                var ratingDistribution = await _context.ProductReviews
                    .Where(r => r.ProductId == productId && r.IsActive && r.IsApproved)
                    .GroupBy(r => r.Rating)
                    .Select(g => new
                    {
                        Rating = g.Key,
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Rating)
                    .ToListAsync(cancellationToken);

                // Sửa lỗi: Tạo đối tượng động với dynamic
                dynamic result = new
                {
                    AverageRating = Math.Round(averageRating, 1),
                    ReviewCount = reviewCount,
                    RatingDistribution = ratingDistribution
                };

                return ApiResponse<object>.Ok(result, "Product review statistics retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Fail($"An error occurred while retrieving review statistics: {ex.Message}", null, 500);
            }
        }

        #region Private Helper Methods

        private async Task<ApiResponse<ReviewResponseDto>> ValidateReviewCreationAsync(string userId, CreateReviewDto createReviewDto, CancellationToken cancellationToken)
        {
            // Check if order exists and belongs to user
            var order = await _orderRepository.GetByIdWithDetailsAsync(createReviewDto.OrderId, cancellationToken);
            if (order == null || order.CustomerId != userId)
                return ApiResponse<ReviewResponseDto>.Fail("Order not found or access denied.", null, 404);

            // SỬA ĐOẠN NÀY - So sánh không phân biệt hoa thường
            if (!order.OrderStatus.Equals("delivered", StringComparison.OrdinalIgnoreCase) ||
                !order.PaymentStatus.Equals("paid", StringComparison.OrdinalIgnoreCase))
                return ApiResponse<ReviewResponseDto>.Fail("You can only review products from delivered and paid orders.", null, 400);

            // Check if product exists in the order
            var productInOrder = order.OrderItems.Any(oi => oi.ProductId == createReviewDto.ProductId);
            if (!productInOrder)
                return ApiResponse<ReviewResponseDto>.Fail("Product not found in the specified order.", null, 400);

            // Check if product exists
            var product = await _productsRepository.GetByIdAsync(createReviewDto.ProductId, cancellationToken);
            if (product == null || !product.IsActive)
                return ApiResponse<ReviewResponseDto>.Fail("Product not found or not available.", null, 400);

            return ApiResponse<ReviewResponseDto>.Ok(null!, "Valid");
        }
        private async Task SaveReviewImagesAsync(int reviewId, List<IFormFile> images, CancellationToken cancellationToken)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "reviews");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var reviewImages = new List<ReviewImages>();

            foreach (var image in images)
            {
                if (image.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream, cancellationToken);
                    }

                    var imageUrl = $"/uploads/reviews/{fileName}";
                    reviewImages.Add(new ReviewImages
                    {
                        ReviewId = reviewId,
                        ImageUrl = imageUrl,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow
                    });
                }
            }

            if (reviewImages.Any())
            {
                await _reviewImageRepository.AddRangeAsync(reviewImages, cancellationToken);
                await _reviewImageRepository.SaveChangesAsync(cancellationToken);
            }
        }

        private async Task DeleteReviewImagesAsync(List<int> imageIds, CancellationToken cancellationToken)
        {
            var images = await _reviewImageRepository.GetAllAsync(cancellationToken);
            var imagesToDelete = images.Where(img => imageIds.Contains(img.Id)).ToList();

            foreach (var image in imagesToDelete)
            {
                // Delete physical file
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, image.ImageUrl.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                // Mark as inactive in database
                image.IsActive = false;
            }

            _reviewImageRepository.UpdateRange(imagesToDelete);
            await _reviewImageRepository.SaveChangesAsync(cancellationToken);
        }

        #endregion
    }
}