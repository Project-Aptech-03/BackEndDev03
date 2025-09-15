using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class BlogLikeRepository : IBlogLikeRepository
    {
        private readonly ApplicationDbContext _context;

        public BlogLikeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BlogLikes?> GetByBlogAndUserAsync(int blogId, string userId)
        {
            return await _context.BlogLikes
                .FirstOrDefaultAsync(bl => bl.BlogId == blogId && bl.UserId == userId);
        }

        public async Task<BlogLikes> CreateAsync(BlogLikes like)
        {
            _context.BlogLikes.Add(like);
            await _context.SaveChangesAsync();
            return like;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var like = await _context.BlogLikes.FindAsync(id);
            if (like == null) return false;

            _context.BlogLikes.Remove(like);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int blogId, string userId)
        {
            return await _context.BlogLikes
                .AnyAsync(bl => bl.BlogId == blogId && bl.UserId == userId);
        }

        public async Task<int> GetLikeCountByBlogIdAsync(int blogId)
        {
            return await _context.BlogLikes
                .CountAsync(bl => bl.BlogId == blogId);
        }

        public async Task<List<BlogLikes>> GetByUserIdAsync(string userId, int page = 1, int pageSize = 10)
        {
            return await _context.BlogLikes
                .Include(bl => bl.Blog)
                .ThenInclude(b => b.Author)
                .Include(bl => bl.Blog)
                .ThenInclude(b => b.Category)
                .Where(bl => bl.UserId == userId)
                .OrderByDescending(bl => bl.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}