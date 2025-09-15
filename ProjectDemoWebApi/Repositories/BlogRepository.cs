using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.DTOs.Blog;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class BlogRepository : IBlogRepository
    {
        private readonly ApplicationDbContext _context;

        public BlogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Blogs?> GetByIdAsync(int id)
        {
            return await _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Comments.Where(c => c.IsApproved))
                .ThenInclude(c => c.User)
                .Include(b => b.Likes)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Blogs?> GetBySlugAsync(string slug)
        {
            return await _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Comments.Where(c => c.IsApproved))
                .ThenInclude(c => c.User)
                .Include(b => b.Likes)
                .FirstOrDefaultAsync(b => b.Slug == slug);
        }

        public async Task<PagedResultDto<Blogs>> GetPagedAsync(BlogQueryDto query)
        {
            var blogsQuery = _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Likes)
                .AsQueryable();

            // Apply filters
            if (query.IsPublished.HasValue)
                blogsQuery = blogsQuery.Where(b => b.IsPublished == query.IsPublished.Value);

            if (query.IsFeatured.HasValue)
                blogsQuery = blogsQuery.Where(b => b.IsFeatured == query.IsFeatured.Value);

            if (query.CategoryId.HasValue)
                blogsQuery = blogsQuery.Where(b => b.CategoryId == query.CategoryId.Value);

            if (!string.IsNullOrEmpty(query.AuthorId))
                blogsQuery = blogsQuery.Where(b => b.AuthorId == query.AuthorId);

            if (!string.IsNullOrEmpty(query.Search))
            {
                blogsQuery = blogsQuery.Where(b =>
                    b.Title.Contains(query.Search) ||
                    b.Content.Contains(query.Search) ||
                    b.Summary.Contains(query.Search));
            }

            // Apply sorting
            blogsQuery = query.SortBy?.ToLower() switch
            {
                "publisheddate" => query.SortOrder == "asc"
                    ? blogsQuery.OrderBy(b => b.PublishedDate)
                    : blogsQuery.OrderByDescending(b => b.PublishedDate),
                "viewcount" => query.SortOrder == "asc"
                    ? blogsQuery.OrderBy(b => b.ViewCount)
                    : blogsQuery.OrderByDescending(b => b.ViewCount),
                "likecount" => query.SortOrder == "asc"
                    ? blogsQuery.OrderBy(b => b.LikeCount)
                    : blogsQuery.OrderByDescending(b => b.LikeCount),
                _ => query.SortOrder == "asc"
                    ? blogsQuery.OrderBy(b => b.CreatedDate)
                    : blogsQuery.OrderByDescending(b => b.CreatedDate)
            };

            var totalCount = await blogsQuery.CountAsync();
            var blogs = await blogsQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PagedResultDto<Blogs>
            {
                Items = blogs,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<List<Blogs>> GetFeaturedAsync(int count = 5)
        {
            return await _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Likes)
                .Where(b => b.IsFeatured && b.IsPublished)
                .OrderByDescending(b => b.PublishedDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Blogs>> GetRecentAsync(int count = 5)
        {
            return await _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Likes)
                .Where(b => b.IsPublished)
                .OrderByDescending(b => b.PublishedDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Blogs>> GetByAuthorAsync(string authorId, int page = 1, int pageSize = 10)
        {
            return await _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Likes)
                .Where(b => b.AuthorId == authorId && b.IsPublished)
                .OrderByDescending(b => b.PublishedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Blogs>> GetByCategoryAsync(int categoryId, int page = 1, int pageSize = 10)
        {
            return await _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Likes)
                .Where(b => b.CategoryId == categoryId && b.IsPublished)
                .OrderByDescending(b => b.PublishedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Blogs> CreateAsync(Blogs blog)
        {
            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();
            return blog;
        }

        public async Task<Blogs> UpdateAsync(Blogs blog)
        {
            blog.UpdatedDate = DateTime.UtcNow;
            _context.Blogs.Update(blog);
            await _context.SaveChangesAsync();
            return blog;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null) return false;

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Blogs.AnyAsync(b => b.Id == id);
        }

        public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
        {
            var query = _context.Blogs.Where(b => b.Slug == slug);
            if (excludeId.HasValue)
                query = query.Where(b => b.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task IncrementViewCountAsync(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                blog.ViewCount++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Blogs.CountAsync();
        }

        public async Task<List<Blogs>> SearchAsync(string searchTerm, int page = 1, int pageSize = 10)
        {
            return await _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Likes)
                .Where(b => b.IsPublished &&
                    (b.Title.Contains(searchTerm) ||
                     b.Content.Contains(searchTerm) ||
                     b.Summary.Contains(searchTerm)))
                .OrderByDescending(b => b.PublishedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}