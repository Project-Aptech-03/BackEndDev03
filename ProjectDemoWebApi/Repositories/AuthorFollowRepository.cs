using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class AuthorFollowRepository : IAuthorFollowRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthorFollowRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AuthorFollows?> GetByFollowerAndAuthorAsync(string followerId, string authorId)
        {
            return await _context.AuthorFollows
                .FirstOrDefaultAsync(af => af.FollowerId == followerId && af.AuthorId == authorId);
        }

        public async Task<AuthorFollows> CreateAsync(AuthorFollows follow)
        {
            _context.AuthorFollows.Add(follow);
            await _context.SaveChangesAsync();
            return follow;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var follow = await _context.AuthorFollows.FindAsync(id);
            if (follow == null) return false;

            _context.AuthorFollows.Remove(follow);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string followerId, string authorId)
        {
            return await _context.AuthorFollows
                .AnyAsync(af => af.FollowerId == followerId && af.AuthorId == authorId);
        }

        public async Task<List<AuthorFollows>> GetFollowersAsync(string authorId, int page = 1, int pageSize = 10)
        {
            return await _context.AuthorFollows
                .Include(af => af.Follower)
                .Where(af => af.AuthorId == authorId)
                .OrderByDescending(af => af.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<AuthorFollows>> GetFollowingAsync(string followerId, int page = 1, int pageSize = 10)
        {
            return await _context.AuthorFollows
                .Include(af => af.Author)
                .Where(af => af.FollowerId == followerId)
                .OrderByDescending(af => af.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetFollowerCountAsync(string authorId)
        {
            return await _context.AuthorFollows
                .CountAsync(af => af.AuthorId == authorId);
        }

        public async Task<int> GetFollowingCountAsync(string followerId)
        {
            return await _context.AuthorFollows
                .CountAsync(af => af.FollowerId == followerId);
        }
    }
}