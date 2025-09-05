using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class ProductPhotosRepository : BaseRepository<ProductPhotos>, IProductPhotosRepository
    {
        public ProductPhotosRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductPhotos>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(p => p.ProductId == productId && p.IsActive)
                .OrderBy(p => p.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<ProductPhotos?> GetByIdAndProductIdAsync(int id, int productId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.Id == id && p.ProductId == productId && p.IsActive, cancellationToken);
        }

        public async Task<bool> DeleteByIdAndProductIdAsync(int id, int productId, CancellationToken cancellationToken = default)
        {
            var photo = await GetByIdAndProductIdAsync(id, productId, cancellationToken);
            if (photo == null) return false;

            // Soft delete
            photo.IsActive = false;
            Update(photo);
            await SaveChangesAsync(cancellationToken);
            
            return true;
        }
    }
}