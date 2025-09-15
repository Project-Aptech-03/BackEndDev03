using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class CommentLikeRepository : ICommentLikeRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentLikeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommentLikes?> GetByCommentAndUserAsync(int commentId, string userId)
        {
            return await _context.CommentLikes
                .FirstOrDefaultAsync(cl => cl.CommentId == commentId && cl.UserId == userId);
        }

        public async Task<CommentLikes> CreateAsync(CommentLikes like)
        {
            _context.CommentLikes.Add(like);
            await _context.SaveChangesAsync();
            return like;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var like = await _context.CommentLikes.FindAsync(id);
            if (like == null) return false;

            _context.CommentLikes.Remove(like);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int commentId, string userId)
        {
            return await _context.CommentLikes
                .AnyAsync(cl => cl.CommentId == commentId && cl.UserId == userId);
        }

        public async Task<int> GetLikeCountByCommentIdAsync(int commentId)
        {
            return await _context.CommentLikes
                .CountAsync(cl => cl.CommentId == commentId);
        }
    }
}