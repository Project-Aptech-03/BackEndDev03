using AutoMapper;
using ProjectDemoWebApi.DTOs.Blog;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Services
{
    public class BlogCommentService : IBlogCommentService
    {
        private readonly IBlogCommentRepository _commentRepository;
        private readonly ICommentLikeRepository _commentLikeRepository;
        private readonly IBlogRepository _blogRepository;
        private readonly IMapper _mapper;

        public BlogCommentService(
            IBlogCommentRepository commentRepository,
            ICommentLikeRepository commentLikeRepository,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            _commentRepository = commentRepository;
            _commentLikeRepository = commentLikeRepository;
            _blogRepository = blogRepository;
            _mapper = mapper;
        }

        public async Task<List<CommentResponseDto>> GetByBlogIdAsync(int blogId, string? currentUserId = null)
        {
            var comments = await _commentRepository.GetByBlogIdAsync(blogId);
            var dtos = new List<CommentResponseDto>();

            foreach (var comment in comments)
            {
                var dto = await MapCommentToDto(comment, currentUserId);
                dtos.Add(dto);
            }

            return dtos;
        }

        public async Task<CommentResponseDto> CreateAsync(int blogId, CreateCommentDto dto, string userId)
        {
            var blog = await _blogRepository.GetByIdAsync(blogId);
            if (blog == null)
                throw new ArgumentException("Blog not found");

            var comment = _mapper.Map<BlogComments>(dto);
            comment.BlogId = blogId;
            comment.UserId = userId;
            comment.CreatedDate = DateTime.UtcNow;
            comment.UpdatedDate = DateTime.UtcNow;

            var createdComment = await _commentRepository.CreateAsync(comment);

            // Update comment count
            blog.CommentCount++;
            await _blogRepository.UpdateAsync(blog);

            return await MapCommentToDto(createdComment, userId);
        }

        public async Task<CommentResponseDto> UpdateAsync(int id, UpdateCommentDto dto, string currentUserId)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
                throw new ArgumentException("Comment not found");

            if (comment.UserId != currentUserId)
                throw new UnauthorizedAccessException("You can only update your own comments");

            _mapper.Map(dto, comment);
            comment.UpdatedDate = DateTime.UtcNow;

            await _commentRepository.UpdateAsync(comment);
            return await MapCommentToDto(comment, currentUserId);
        }

        public async Task<bool> DeleteAsync(int id, string currentUserId)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null) return false;

            if (comment.UserId != currentUserId)
                throw new UnauthorizedAccessException("You can only delete your own comments");

            var result = await _commentRepository.DeleteAsync(id);

            if (result)
            {
                var blog = await _blogRepository.GetByIdAsync(comment.BlogId);
                if (blog != null)
                {
                    blog.CommentCount = Math.Max(0, blog.CommentCount - 1);
                    await _blogRepository.UpdateAsync(blog);
                }
            }
            return result;
        }

        public async Task<bool> LikeCommentAsync(int commentId, string userId)
        {
            var exists = await _commentLikeRepository.ExistsAsync(commentId, userId);
            if (exists) return false;

            var like = new CommentLikes
            {
                CommentId = commentId,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };

            await _commentLikeRepository.CreateAsync(like);

            // Update like count
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment != null)
            {
                comment.LikeCount++;
                await _commentRepository.UpdateAsync(comment);
            }

            return true;
        }

        public async Task<bool> UnlikeCommentAsync(int commentId, string userId)
        {
            var like = await _commentLikeRepository.GetByCommentAndUserAsync(commentId, userId);
            if (like == null) return false;

            await _commentLikeRepository.DeleteAsync(like.Id);

            // Update like count
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment != null)
            {
                comment.LikeCount = Math.Max(0, comment.LikeCount - 1);
                await _commentRepository.UpdateAsync(comment);
            }

            return true;
        }

        private async Task<CommentResponseDto> MapCommentToDto(BlogComments comment, string? currentUserId)
        {
            var dto = _mapper.Map<CommentResponseDto>(comment);
            dto.UserName = $"{comment.User.FirstName} {comment.User.LastName}";
            dto.UserAvatar = comment.User.AvatarUrl;
            dto.IsLikedByCurrentUser = !string.IsNullOrEmpty(currentUserId) &&
                await _commentLikeRepository.ExistsAsync(comment.Id, currentUserId);

            // Map replies
            if (comment.Replies.Any())
            {
                dto.Replies = new List<CommentResponseDto>();
                foreach (var reply in comment.Replies)
                {
                    var replyDto = await MapCommentToDto(reply, currentUserId);
                    dto.Replies.Add(replyDto);
                }
            }

            return dto;
        }
    }
}