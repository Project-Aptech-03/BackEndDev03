using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectDemoWebApi.DTOs.Blog;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.Services.Interface;
using System.Security.Claims;

namespace ProjectDemoWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly IBlogCommentService _commentService;
        private readonly IAuthorFollowService _authorFollowService;

        public BlogController(
            IBlogService blogService,
            IBlogCommentService commentService,
            IAuthorFollowService authorFollowService)
        {
            _blogService = blogService;
            _commentService = commentService;
            _authorFollowService = authorFollowService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<BlogListResponseDto>>> GetBlogs([FromQuery] BlogQueryDto query)
        {
            var currentUserId = GetCurrentUserId();
            var result = await _blogService.GetPagedAsync(query, currentUserId);
            return Ok(result);
        }

        [HttpGet("featured")]
        public async Task<ActionResult<List<BlogListResponseDto>>> GetFeaturedBlogs([FromQuery] int count = 5)
        {
            var currentUserId = GetCurrentUserId();
            var blogs = await _blogService.GetFeaturedAsync(count, currentUserId);
            return Ok(blogs);
        }

        [HttpGet("recent")]
        public async Task<ActionResult<List<BlogListResponseDto>>> GetRecentBlogs([FromQuery] int count = 5)
        {
            var currentUserId = GetCurrentUserId();
            var blogs = await _blogService.GetRecentAsync(count, currentUserId);
            return Ok(blogs);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<BlogListResponseDto>>> SearchBlogs(
            [FromQuery] string searchTerm,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Search term is required");

            var currentUserId = GetCurrentUserId();
            var blogs = await _blogService.SearchAsync(searchTerm, page, pageSize, currentUserId);
            return Ok(blogs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BlogResponseDto>> GetBlog(int id)
        {
            var currentUserId = GetCurrentUserId();
            var blog = await _blogService.GetByIdAsync(id, currentUserId);

            if (blog == null)
                return NotFound("Blog not found");

            // Increment view count
            await _blogService.IncrementViewCountAsync(id);

            return Ok(blog);
        }

        [HttpGet("slug/{slug}")]
        public async Task<ActionResult<BlogResponseDto>> GetBlogBySlug(string slug)
        {
            var currentUserId = GetCurrentUserId();
            var blog = await _blogService.GetBySlugAsync(slug, currentUserId);

            if (blog == null)
                return NotFound("Blog not found");

            // Increment view count
            await _blogService.IncrementViewCountAsync(blog.Id);

            return Ok(blog);
        }

        [HttpGet("author/{authorId}")]
        public async Task<ActionResult<List<BlogListResponseDto>>> GetBlogsByAuthor(
            string authorId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var currentUserId = GetCurrentUserId();
            var blogs = await _blogService.GetByAuthorAsync(authorId, page, pageSize, currentUserId);
            return Ok(blogs);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<List<BlogListResponseDto>>> GetBlogsByCategory(
            int categoryId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var currentUserId = GetCurrentUserId();
            var blogs = await _blogService.GetByCategoryAsync(categoryId, page, pageSize, currentUserId);
            return Ok(blogs);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<BlogResponseDto>> CreateBlog([FromBody] CreateBlogDto dto)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            try
            {
                var blog = await _blogService.CreateAsync(dto, currentUserId);
                return CreatedAtAction(nameof(GetBlog), new { id = blog.Id }, blog);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<BlogResponseDto>> UpdateBlog(int id, [FromBody] UpdateBlogDto dto)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            try
            {
                var blog = await _blogService.UpdateAsync(id, dto, currentUserId);
                return Ok(blog);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteBlog(int id)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            try
            {
                var result = await _blogService.DeleteAsync(id, currentUserId);
                if (!result)
                    return NotFound("Blog not found");

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpPost("{id}/like")]
        [Authorize]
        public async Task<ActionResult> LikeBlog(int id)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var result = await _blogService.LikeBlogAsync(id, currentUserId);
            if (!result)
                return BadRequest("Already liked or blog not found");

            return Ok(new { message = "Blog liked successfully" });
        }

        [HttpDelete("{id}/like")]
        [Authorize]
        public async Task<ActionResult> UnlikeBlog(int id)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var result = await _blogService.UnlikeBlogAsync(id, currentUserId);
            if (!result)
                return BadRequest("Like not found");

            return Ok(new { message = "Blog unliked successfully" });
        }

        // Comment endpoints
        [HttpGet("{id}/comments")]
        public async Task<ActionResult<List<CommentResponseDto>>> GetBlogComments(int id)
        {
            var currentUserId = GetCurrentUserId();
            var comments = await _commentService.GetByBlogIdAsync(id, currentUserId);
            return Ok(comments);
        }

        [HttpPost("{id}/comments")]
        [Authorize]
        public async Task<ActionResult<CommentResponseDto>> CreateComment(int id, [FromBody] CreateCommentDto dto)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            try
            {
                var comment = await _commentService.CreateAsync(id, dto, currentUserId);
                return CreatedAtAction(nameof(GetBlogComments), new { id }, comment);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("comments/{commentId}")]
        [Authorize]
        public async Task<ActionResult<CommentResponseDto>> UpdateComment(int commentId, [FromBody] UpdateCommentDto dto)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            try
            {
                var comment = await _commentService.UpdateAsync(commentId, dto, currentUserId);
                return Ok(comment);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("comments/{commentId}")]
        [Authorize]
        public async Task<ActionResult> DeleteComment(int commentId)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            try
            {
                var result = await _commentService.DeleteAsync(commentId, currentUserId);
                if (!result)
                    return NotFound("Comment not found");

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpPost("comments/{commentId}/like")]
        [Authorize]
        public async Task<ActionResult> LikeComment(int commentId)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var result = await _commentService.LikeCommentAsync(commentId, currentUserId);
            if (!result)
                return BadRequest("Already liked or comment not found");

            return Ok(new { message = "Comment liked successfully" });
        }

        [HttpDelete("comments/{commentId}/like")]
        [Authorize]
        public async Task<ActionResult> UnlikeComment(int commentId)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var result = await _commentService.UnlikeCommentAsync(commentId, currentUserId);
            if (!result)
                return BadRequest("Like not found");

            return Ok(new { message = "Comment unliked successfully" });
        }

        // Author follow endpoints
        [HttpGet("authors/{authorId}/followers")]
        public async Task<ActionResult<List<AuthorFollowDto>>> GetAuthorFollowers(
            string authorId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var currentUserId = GetCurrentUserId();
            var followers = await _authorFollowService.GetFollowersAsync(authorId, page, pageSize, currentUserId);
            return Ok(followers);
        }

        [HttpGet("authors/{authorId}/following")]
        public async Task<ActionResult<List<AuthorFollowDto>>> GetAuthorFollowing(
            string authorId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var currentUserId = GetCurrentUserId();
            var following = await _authorFollowService.GetFollowingAsync(authorId, page, pageSize, currentUserId);
            return Ok(following);
        }

        [HttpPost("authors/{authorId}/follow")]
        [Authorize]
        public async Task<ActionResult> FollowAuthor(string authorId)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var result = await _authorFollowService.FollowAuthorAsync(authorId, currentUserId);
            if (!result)
                return BadRequest("Cannot follow this author or already following");

            return Ok(new { message = "Author followed successfully" });
        }

        [HttpDelete("authors/{authorId}/follow")]
        [Authorize]
        public async Task<ActionResult> UnfollowAuthor(string authorId)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var result = await _authorFollowService.UnfollowAuthorAsync(authorId, currentUserId);
            if (!result)
                return BadRequest("Not following this author");

            return Ok(new { message = "Author unfollowed successfully" });
        }

        [HttpGet("authors/suggested")]
        [Authorize]
        public async Task<ActionResult<List<AuthorFollowDto>>> GetSuggestedAuthors([FromQuery] int count = 5)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var suggestedAuthors = await _authorFollowService.GetSuggestedAuthorsAsync(currentUserId, count);
            return Ok(suggestedAuthors);
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}