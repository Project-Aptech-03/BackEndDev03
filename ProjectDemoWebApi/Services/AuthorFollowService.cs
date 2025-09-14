using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.DTOs.Blog;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Services
{
    public class AuthorFollowService : IAuthorFollowService
    {
        private readonly IAuthorFollowRepository _authorFollowRepository;
        private readonly IBlogRepository _blogRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AuthorFollowService(
            IAuthorFollowRepository authorFollowRepository,
            IBlogRepository blogRepository,
            ApplicationDbContext context,
            IMapper mapper)
        {
            _authorFollowRepository = authorFollowRepository;
            _blogRepository = blogRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<AuthorFollowDto>> GetFollowersAsync(string authorId, int page = 1, int pageSize = 10, string? currentUserId = null)
        {
            var follows = await _authorFollowRepository.GetFollowersAsync(authorId, page, pageSize);
            var dtos = new List<AuthorFollowDto>();

            foreach (var follow in follows)
            {
                var dto = new AuthorFollowDto
                {
                    AuthorId = follow.FollowerId,
                    AuthorName = $"{follow.Follower.FirstName} {follow.Follower.LastName}",
                    AuthorAvatar = follow.Follower.AvatarUrl,
                    BlogCount = await GetBlogCountAsync(follow.FollowerId),
                    FollowerCount = await _authorFollowRepository.GetFollowerCountAsync(follow.FollowerId),
                    IsFollowing = !string.IsNullOrEmpty(currentUserId) && 
                        await _authorFollowRepository.ExistsAsync(currentUserId, follow.FollowerId)
                };
                dtos.Add(dto);
            }

            return dtos;
        }

        public async Task<List<AuthorFollowDto>> GetFollowingAsync(string followerId, int page = 1, int pageSize = 10, string? currentUserId = null)
        {
            var follows = await _authorFollowRepository.GetFollowingAsync(followerId, page, pageSize);
            var dtos = new List<AuthorFollowDto>();

            foreach (var follow in follows)
            {
                var dto = new AuthorFollowDto
                {
                    AuthorId = follow.AuthorId,
                    AuthorName = $"{follow.Author.FirstName} {follow.Author.LastName}",
                    AuthorAvatar = follow.Author.AvatarUrl,
                    BlogCount = await GetBlogCountAsync(follow.AuthorId),
                    FollowerCount = await _authorFollowRepository.GetFollowerCountAsync(follow.AuthorId),
                    IsFollowing = !string.IsNullOrEmpty(currentUserId) && 
                        await _authorFollowRepository.ExistsAsync(currentUserId, follow.AuthorId)
                };
                dtos.Add(dto);
            }

            return dtos;
        }

        public async Task<bool> FollowAuthorAsync(string authorId, string followerId)
        {
            if (authorId == followerId)
                return false; // Cannot follow yourself

            var exists = await _authorFollowRepository.ExistsAsync(followerId, authorId);
            if (exists) return false;

            var follow = new AuthorFollows
            {
                FollowerId = followerId,
                AuthorId = authorId,
                CreatedDate = DateTime.UtcNow
            };

            await _authorFollowRepository.CreateAsync(follow);
            return true;
        }

        public async Task<bool> UnfollowAuthorAsync(string authorId, string followerId)
        {
            var follow = await _authorFollowRepository.GetByFollowerAndAuthorAsync(followerId, authorId);
            if (follow == null) return false;

            await _authorFollowRepository.DeleteAsync(follow.Id);
            return true;
        }

        public async Task<bool> IsFollowingAsync(string authorId, string followerId)
        {
            return await _authorFollowRepository.ExistsAsync(followerId, authorId);
        }

        public async Task<List<AuthorFollowDto>> GetSuggestedAuthorsAsync(string currentUserId, int count = 5)
        {
            // Get authors that the current user is not following
            var followingIds = await _context.AuthorFollows
                .Where(af => af.FollowerId == currentUserId)
                .Select(af => af.AuthorId)
                .ToListAsync();

            var suggestedAuthors = await _context.Users
                .Where(u => u.Id != currentUserId && !followingIds.Contains(u.Id))
                .Where(u => u.Blogs.Any(b => b.IsPublished)) // Only authors with published blogs
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.AvatarUrl,
                    BlogCount = u.Blogs.Count(b => b.IsPublished),
                    FollowerCount = u.Followers.Count
                })
                .OrderByDescending(u => u.FollowerCount)
                .ThenByDescending(u => u.BlogCount)
                .Take(count)
                .ToListAsync();

            var dtos = suggestedAuthors.Select(author => new AuthorFollowDto
            {
                AuthorId = author.Id,
                AuthorName = $"{author.FirstName} {author.LastName}",
                AuthorAvatar = author.AvatarUrl,
                BlogCount = author.BlogCount,
                FollowerCount = author.FollowerCount,
                IsFollowing = false
            }).ToList();

            return dtos;
        }

        private async Task<int> GetBlogCountAsync(string userId)
        {
            return await _context.Blogs
                .CountAsync(b => b.AuthorId == userId && b.IsPublished);
        }
    }
}
