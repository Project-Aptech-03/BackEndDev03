using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class ManufacturerRepository : BaseRepository<Manufacturers>, IManufacturerRepository
    {
        public ManufacturerRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Manufacturers?> GetByManufacturerCodeAsync(string manufacturerCode, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(m => m.ManufacturerCode == manufacturerCode, cancellationToken);
        }

        public async Task<IEnumerable<Manufacturers>> GetActiveManufacturersAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(m => m.IsActive)
                .OrderBy(m => m.ManufacturerName)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsManufacturerCodeExistsAsync(string manufacturerCode, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable();
            
            if (excludeId.HasValue)
                query = query.Where(m => m.Id != excludeId.Value);
                
            return await query.AnyAsync(m => m.ManufacturerCode == manufacturerCode, cancellationToken);
        }
    }
}