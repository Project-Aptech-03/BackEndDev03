using AutoMapper;
using ProjectDemoWebApi.DTOs.Blog;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;
using System.Text.RegularExpressions;

namespace ProjectDemoWebApi.Services
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IBlogLikeRepository _blogLikeRepository;
        private readonly IBlogCommentRepository _blogCommentRepository;
        private readonly IMapper _mapper;

        public BlogService(
            IBlogRepository blogRepository,
            IBlogLikeRepository blogLikeRepository,
            IBlogCommentRepository blogCommentRepository,
            IMapper mapper)
        {
            _blogRepository = blogRepository;
            _blogLikeRepository = blogLikeRepository;
            _blogCommentRepository = blogCommentRepository;
            _mapper = mapper;
        }

        public async Task<BlogResponseDto?> GetByIdAsync(int id, string? currentUserId = null)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null) return null;

            var dto = _mapper.Map<BlogResponseDto>(blog);
            dto.AuthorName = $"{blog.Author.FirstName} {blog.Author.LastName}";
            dto.AuthorAvatar = blog.Author.AvatarUrl;
            dto.CategoryName = blog.Category.CategoryName;
            dto.IsLikedByCurrentUser = !string.IsNullOrEmpty(currentUserId) &&
                await _blogLikeRepository.ExistsAsync(id, currentUserId);

            return dto;
        }

        public async Task<BlogResponseDto?> GetBySlugAsync(string slug, string? currentUserId = null)
        {
            var blog = await _blogRepository.GetBySlugAsync(slug);
            if (blog == null) return null;

            var dto = _mapper.Map<BlogResponseDto>(blog);
            dto.AuthorName = $"{blog.Author.FirstName} {blog.Author.LastName}";
            dto.AuthorAvatar = blog.Author.AvatarUrl;
            dto.CategoryName = blog.Category.CategoryName;
            dto.IsLikedByCurrentUser = !string.IsNullOrEmpty(currentUserId) &&
                await _blogLikeRepository.ExistsAsync(blog.Id, currentUserId);

            return dto;
        }

        public async Task<PagedResultDto<BlogListResponseDto>> GetPagedAsync(BlogQueryDto query, string? currentUserId = null)
        {
            var result = await _blogRepository.GetPagedAsync(query);
            var dtos = new List<BlogListResponseDto>();

            foreach (var blog in result.Items)
            {
                var dto = _mapper.Map<BlogListResponseDto>(blog);
                dto.AuthorName = $"{blog.Author.FirstName} {blog.Author.LastName}";
                dto.AuthorAvatar = blog.Author.AvatarUrl;
                dto.CategoryName = blog.Category.CategoryName;
                dto.IsLikedByCurrentUser = !string.IsNullOrEmpty(currentUserId) &&
                    await _blogLikeRepository.ExistsAsync(blog.Id, currentUserId);
                dtos.Add(dto);
            }

            return new PagedResultDto<BlogListResponseDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<List<BlogListResponseDto>> GetFeaturedAsync(int count = 5, string? currentUserId = null)
        {
            var blogs = await _blogRepository.GetFeaturedAsync(count);
            var dtos = new List<BlogListResponseDto>();

            foreach (var blog in blogs)
            {
                var dto = _mapper.Map<BlogListResponseDto>(blog);
                dto.AuthorName = $"{blog.Author.FirstName} {blog.Author.LastName}";
                dto.AuthorAvatar = blog.Author.AvatarUrl;
                dto.CategoryName = blog.Category.CategoryName;
                dto.IsLikedByCurrentUser = !string.IsNullOrEmpty(currentUserId) &&
                    await _blogLikeRepository.ExistsAsync(blog.Id, currentUserId);
                dtos.Add(dto);
            }

            return dtos;
        }

        public async Task<List<BlogListResponseDto>> GetRecentAsync(int count = 5, string? currentUserId = null)
        {
            var blogs = await _blogRepository.GetRecentAsync(count);
            var dtos = new List<BlogListResponseDto>();

            foreach (var blog in blogs)
            {
                var dto = _mapper.Map<BlogListResponseDto>(blog);
                dto.AuthorName = $"{blog.Author.FirstName} {blog.Author.LastName}";
                dto.AuthorAvatar = blog.Author.AvatarUrl;
                dto.CategoryName = blog.Category.CategoryName;
                dto.IsLikedByCurrentUser = !string.IsNullOrEmpty(currentUserId) &&
                    await _blogLikeRepository.ExistsAsync(blog.Id, currentUserId);
                dtos.Add(dto);
            }

            return dtos;
        }

        public async Task<List<BlogListResponseDto>> GetByAuthorAsync(string authorId, int page = 1, int pageSize = 10, string? currentUserId = null)
        {
            var blogs = await _blogRepository.GetByAuthorAsync(authorId, page, pageSize);
            var dtos = new List<BlogListResponseDto>();

            foreach (var blog in blogs)
            {
                var dto = _mapper.Map<BlogListResponseDto>(blog);
                dto.AuthorName = $"{blog.Author.FirstName} {blog.Author.LastName}";
                dto.AuthorAvatar = blog.Author.AvatarUrl;
                dto.CategoryName = blog.Category.CategoryName;
                dto.IsLikedByCurrentUser = !string.IsNullOrEmpty(currentUserId) &&
                    await _blogLikeRepository.ExistsAsync(blog.Id, currentUserId);
                dtos.Add(dto);
            }

            return dtos;
        }

        public async Task<List<BlogListResponseDto>> GetByCategoryAsync(int categoryId, int page = 1, int pageSize = 10, string? currentUserId = null)
        {
            var blogs = await _blogRepository.GetByCategoryAsync(categoryId, page, pageSize);
            var dtos = new List<BlogListResponseDto>();

            foreach (var blog in blogs)
            {
                var dto = _mapper.Map<BlogListResponseDto>(blog);
                dto.AuthorName = $"{blog.Author.FirstName} {blog.Author.LastName}";
                dto.AuthorAvatar = blog.Author.AvatarUrl;
                dto.CategoryName = blog.Category.CategoryName;
                dto.IsLikedByCurrentUser = !string.IsNullOrEmpty(currentUserId) &&
                    await _blogLikeRepository.ExistsAsync(blog.Id, currentUserId);
                dtos.Add(dto);
            }

            return dtos;
        }

        public async Task<BlogResponseDto> CreateAsync(CreateBlogDto dto, string authorId)
        {
            var blog = _mapper.Map<Blogs>(dto);
            blog.AuthorId = authorId;
            blog.CreatedDate = DateTime.UtcNow;
            blog.UpdatedDate = DateTime.UtcNow;

            // Generate slug if not provided
            if (string.IsNullOrEmpty(blog.Slug))
            {
                blog.Slug = GenerateSlug(blog.Title);
            }

            // Set published date if publishing
            if (blog.IsPublished && !blog.PublishedDate.HasValue)
            {
                blog.PublishedDate = DateTime.UtcNow;
            }

            var createdBlog = await _blogRepository.CreateAsync(blog);
            return await GetByIdAsync(createdBlog.Id, authorId) ??
                throw new InvalidOperationException("Failed to retrieve created blog");
        }

        public async Task<BlogResponseDto> UpdateAsync(int id, UpdateBlogDto dto, string currentUserId)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null)
                throw new ArgumentException("Blog not found");

            if (blog.AuthorId != currentUserId)
                throw new UnauthorizedAccessException("You can only update your own blogs");

            _mapper.Map(dto, blog);
            blog.UpdatedDate = DateTime.UtcNow;

            // Update published date if publishing for the first time
            if (blog.IsPublished && !blog.PublishedDate.HasValue)
            {
                blog.PublishedDate = DateTime.UtcNow;
            }

            await _blogRepository.UpdateAsync(blog);
            return await GetByIdAsync(id, currentUserId) ??
                throw new InvalidOperationException("Failed to retrieve updated blog");
        }

        public async Task<bool> DeleteAsync(int id, string currentUserId)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null) return false;

            if (blog.AuthorId != currentUserId)
                throw new UnauthorizedAccessException("You can only delete your own blogs");

            return await _blogRepository.DeleteAsync(id);
        }

        public async Task<bool> LikeBlogAsync(int blogId, string userId)
        {
            var exists = await _blogLikeRepository.ExistsAsync(blogId, userId);
            if (exists) return false;

            var like = new BlogLikes
            {
                BlogId = blogId,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };

            await _blogLikeRepository.CreateAsync(like);

            // Update like count
            var blog = await _blogRepository.GetByIdAsync(blogId);
            if (blog != null)
            {
                blog.LikeCount++;
                await _blogRepository.UpdateAsync(blog);
            }

            return true;
        }

        public async Task<bool> UnlikeBlogAsync(int blogId, string userId)
        {
            var like = await _blogLikeRepository.GetByBlogAndUserAsync(blogId, userId);
            if (like == null) return false;

            await _blogLikeRepository.DeleteAsync(like.Id);

            // Update like count
            var blog = await _blogRepository.GetByIdAsync(blogId);
            if (blog != null)
            {
                blog.LikeCount = Math.Max(0, blog.LikeCount - 1);
                await _blogRepository.UpdateAsync(blog);
            }

            return true;
        }

        public async Task IncrementViewCountAsync(int id)
        {
            await _blogRepository.IncrementViewCountAsync(id);
        }

        public async Task<List<BlogListResponseDto>> SearchAsync(string searchTerm, int page = 1, int pageSize = 10, string? currentUserId = null)
        {
            var blogs = await _blogRepository.SearchAsync(searchTerm, page, pageSize);
            var dtos = new List<BlogListResponseDto>();

            foreach (var blog in blogs)
            {
                var dto = _mapper.Map<BlogListResponseDto>(blog);
                dto.AuthorName = $"{blog.Author.FirstName} {blog.Author.LastName}";
                dto.AuthorAvatar = blog.Author.AvatarUrl;
                dto.CategoryName = blog.Category.CategoryName;
                dto.IsLikedByCurrentUser = !string.IsNullOrEmpty(currentUserId) &&
                    await _blogLikeRepository.ExistsAsync(blog.Id, currentUserId);
                dtos.Add(dto);
            }

            return dtos;
        }

        private string GenerateSlug(string title)
        {
            // Convert to lowercase and replace spaces with hyphens
            var slug = title.ToLowerInvariant();
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-");
            slug = slug.Trim('-');

            // Ensure uniqueness
            var baseSlug = slug;
            var counter = 1;
            while (_blogRepository.SlugExistsAsync(slug).Result)
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            return slug;
        }
    }
}