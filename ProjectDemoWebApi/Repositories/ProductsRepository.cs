using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class ProductsRepository : BaseRepository<Products>, IProductsRepository
    {
        public ProductsRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Products?> GetByProductCodeAsync(string productCode, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Publisher)
                .Include(p => p.ProductPhotos.Where(ph => ph.IsActive))
                .FirstOrDefaultAsync(p => p.ProductCode == productCode, cancellationToken);
        }

        public async Task<Products?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Publisher)
                .Include(p => p.ProductPhotos.Where(ph => ph.IsActive))
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Products>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Publisher)
                .Include(p => p.ProductPhotos.Where(ph => ph.IsActive))
                .Where(p => p.IsActive) // Only return active products by default
                .OrderBy(p => p.ProductName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Products>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Publisher)
                .Include(p => p.ProductPhotos.Where(ph => ph.IsActive))
                .Where(p => p.IsActive && p.StockQuantity > 0) // Include stock check
                .OrderBy(p => p.ProductName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Products>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Publisher)
                .Include(p => p.ProductPhotos.Where(ph => ph.IsActive))
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
                .Include(p => p.ProductPhotos.Where(ph => ph.IsActive))
                .Where(p => p.ManufacturerId == manufacturerId && p.IsActive && p.StockQuantity > 0)
                .OrderBy(p => p.ProductName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Products>> GetByPublisherAsync(int publisherId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Publisher)
                .Include(p => p.ProductPhotos.Where(ph => ph.IsActive))
                .Where(p => p.PublisherId == publisherId && p.IsActive && p.StockQuantity > 0)
                .OrderBy(p => p.ProductName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Products>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            var lowerSearchTerm = searchTerm.ToLower(); // Pre-convert to lower case
            
            return await _dbSet.AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Publisher)
                .Include(p => p.ProductPhotos.Where(ph => ph.IsActive))
                .Where(p => p.IsActive && p.StockQuantity > 0 &&
                           (EF.Functions.Like(p.ProductName.ToLower(), $"%{lowerSearchTerm}%") || 
                            EF.Functions.Like(p.ProductCode.ToLower(), $"%{lowerSearchTerm}%") ||
                            (p.Author != null && EF.Functions.Like(p.Author.ToLower(), $"%{lowerSearchTerm}%")) ||
                            (p.Description != null && EF.Functions.Like(p.Description.ToLower(), $"%{lowerSearchTerm}%"))))
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

        public async Task<IEnumerable<Products>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Where(p => p.IsActive && p.StockQuantity <= threshold)
                .OrderBy(p => p.StockQuantity)
                .ThenBy(p => p.ProductName)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateStockAsync(int productId, int newStock, CancellationToken cancellationToken = default)
        {
            await _dbSet
                .Where(p => p.Id == productId)
                .ExecuteUpdateAsync(p => p.SetProperty(x => x.StockQuantity, newStock), cancellationToken);
        }
    }
}