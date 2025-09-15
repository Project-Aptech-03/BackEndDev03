using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class BlogCommentRepository : IBlogCommentRepository
    {
        private readonly ApplicationDbContext _context;

        public BlogCommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BlogComments?> GetByIdAsync(int id)
        {
            return await _context.BlogComments
                .Include(c => c.User)
                .Include(c => c.Blog)
                .Include(c => c.ParentComment)
                .Include(c => c.Replies.Where(r => r.IsApproved))
                .ThenInclude(r => r.User)
                .Include(c => c.Likes)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<BlogComments>> GetByBlogIdAsync(int blogId)
        {
            return await _context.BlogComments
                .Include(c => c.User)
                .Include(c => c.Replies.Where(r => r.IsApproved))
                .ThenInclude(r => r.User)
                .Include(c => c.Likes)
                .Where(c => c.BlogId == blogId && c.IsApproved && c.ParentCommentId == null)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<BlogComments>> GetByUserIdAsync(string userId, int page = 1, int pageSize = 10)
        {
            return await _context.BlogComments
                .Include(c => c.User)
                .Include(c => c.Blog)
                .Include(c => c.Likes)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<BlogComments> CreateAsync(BlogComments comment)
        {
            _context.BlogComments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<BlogComments> UpdateAsync(BlogComments comment)
        {
            comment.UpdatedDate = DateTime.UtcNow;
            _context.BlogComments.Update(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var comment = await _context.BlogComments.FindAsync(id);
            if (comment == null) return false;

            _context.BlogComments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.BlogComments.AnyAsync(c => c.Id == id);
        }

        public async Task<int> GetCommentCountByBlogIdAsync(int blogId)
        {
            return await _context.BlogComments
                .Where(c => c.BlogId == blogId && c.IsApproved)
                .CountAsync();
        }

        public async Task<List<BlogComments>> GetRepliesAsync(int parentCommentId)
        {
            return await _context.BlogComments
                .Include(c => c.User)
                .Include(c => c.Likes)
                .Where(c => c.ParentCommentId == parentCommentId && c.IsApproved)
                .OrderBy(c => c.CreatedDate)
                .ToListAsync();
        }
    }
}