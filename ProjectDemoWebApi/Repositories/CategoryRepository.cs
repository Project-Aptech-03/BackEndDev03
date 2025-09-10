using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class CategoryRepository : BaseRepository<Categories>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Categories?> GetByCategoryCodeAsync(string categoryCode, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryCode == categoryCode, cancellationToken);
        }
        public async Task<IEnumerable<Categories>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.CategoryName)
                .ToListAsync(cancellationToken);
        }
        public async Task<(IEnumerable<Categories> Items, int TotalCount)> GetActiveCategoriesPagedAsync(
     int pageNumber = 1,
     int pageSize = 10,
     CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.CategoryName);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<bool> IsCategoryCodeExistsAsync(string categoryCode, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable();
            
            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);
                
            return await query.AnyAsync(c => c.CategoryCode == categoryCode, cancellationToken);
        }
        
        //public async Task<Categories> GetCategoriesAsync(CancellationToken cancellationToken = default)
        //{

        //}
    }
}