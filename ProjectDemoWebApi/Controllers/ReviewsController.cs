using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectDemoWebApi.DTOs.Review;
using ProjectDemoWebApi.Services.Interface;
using System.Security.Claims;

namespace ProjectDemoWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User not authenticated");
        }

        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        /// <summary>
        /// Create a new review for an order product
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateReview([FromForm] CreateReviewDto createReviewDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _reviewService.CreateReviewAsync(userId, createReviewDto, cancellationToken);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Update a review
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(int id, [FromForm] UpdateReviewDto updateReviewDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _reviewService.UpdateReviewAsync(id, userId, updateReviewDto, cancellationToken);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Delete a review
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var result = await _reviewService.DeleteReviewAsync(id, userId, cancellationToken);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get review by ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewById(int id, CancellationToken cancellationToken)
        {
            var result = await _reviewService.GetReviewByIdAsync(id, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get reviews by product ID
        /// </summary>
        [HttpGet("product/{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsByProductId(int productId, CancellationToken cancellationToken)
        {
            var result = await _reviewService.GetReviewsByProductIdAsync(productId, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get reviews by order ID
        /// </summary>
        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetReviewsByOrderId(int orderId, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var result = await _reviewService.GetReviewsByOrderIdAsync(orderId, userId, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get current user's reviews
        /// </summary>
        [HttpGet("my-reviews")]
        public async Task<IActionResult> GetMyReviews(CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var result = await _reviewService.GetCustomerReviewsAsync(userId, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get product review statistics
        /// </summary>
        [HttpGet("product/{productId}/stats")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductReviewStats(int productId, CancellationToken cancellationToken)
        {
            var result = await _reviewService.GetProductReviewStatsAsync(productId, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Add a reply to a review
        /// </summary>
        [HttpPost("{reviewId}/replies")]
        public async Task<IActionResult> AddReply(int reviewId, [FromBody] CreateReplyDto createReplyDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _reviewService.AddReplyAsync(reviewId, userId, createReplyDto, cancellationToken);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Delete a reply
        /// </summary>
        [HttpDelete("replies/{replyId}")]
        public async Task<IActionResult> DeleteReply(int replyId, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var result = await _reviewService.DeleteReplyAsync(replyId, userId, cancellationToken);

            return StatusCode(result.StatusCode, result);
        }

        #region Admin Functions

        [HttpGet("pending")]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> GetPendingReviews(CancellationToken cancellationToken)
        {
            var result = await _reviewService.GetPendingReviewsAsync(cancellationToken);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Approve a review (Admin only)
        /// </summary>
        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> ApproveReview(int id, CancellationToken cancellationToken)
        {
            var result = await _reviewService.ApproveReviewAsync(id, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Reject a review (Admin only)
        /// </summary>
        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> RejectReview(int id, [FromBody] string reason, CancellationToken cancellationToken)
        {
            var result = await _reviewService.RejectReviewAsync(id, reason, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }

        #endregion
    }
}