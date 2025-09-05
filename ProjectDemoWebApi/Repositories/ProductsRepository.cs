using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class ProductsRepository : BaseRepository<Products>, IProductsRepository
    {
        public ProductsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Products>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Publisher)
                .Include(p => p.ProductPhotos.Where(ph => ph.IsActive)) // Ch? l?y ?nh active
                .Where(p => p.IsActive) // Only return active products by default
                .OrderBy(p => p.ProductName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Products>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Publisher)
                .Include(p => p.ProductPhotos.Where(ph => ph.IsActive)) // Ch? l?y ?nh active
                .Where(p => p.CategoryId == categoryId && p.IsActive && p.StockQuantity > 0)
                .OrderBy(p => p.ProductName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Products>> GetByManufacturerAsync(int manufacturerId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Publisher)
                .Include(p => p.ProductPhotos.Where(ph => ph.IsActive)) // Ch? l?y ?nh active
                .Where(p => p.ManufacturerId == manufacturerId && p.IsActive && p.StockQuantity > 0)
                .OrderBy(p => p.ProductName)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsProductCodeExistsAsync(string productCode, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable();
            
            if (excludeId.HasValue)
                query = query.Where(p => p.Id != excludeId.Value);
                
            return await query.AnyAsync(p => p.ProductCode == productCode, cancellationToken);
        }

        public async Task UpdateStockAsync(int productId, int newStock, CancellationToken cancellationToken = default)
        {
            await _dbSet
                .Where(p => p.Id == productId)
                .ExecuteUpdateAsync(p => p.SetProperty(x => x.StockQuantity, newStock), cancellationToken);
        }

        public async Task<Products?> GetByIdNoTrackingAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Manufacturer) 
                .Include(p => p.Publisher)
                .Include(p => p.ProductPhotos.Where(ph => ph.IsActive)) // Include photos khi get by ID
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Products>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.ProductPhotos.Where(ph => ph.IsActive)) // Include photos cho low stock
                .Where(p => p.IsActive && p.StockQuantity <= threshold)
                .OrderBy(p => p.StockQuantity)
                .ThenBy(p => p.ProductName)
                .ToListAsync(cancellationToken);
        }
    }
}