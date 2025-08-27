using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Products>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Products
             .Include(p => p.ProductPhotos)
             .AsNoTracking()
             .ToListAsync(cancellationToken);
        }

        public async Task<Products?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                 .Include(p => p.ProductPhotos)
                 .AsNoTracking()
                 .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task AddAsync(Products products, CancellationToken cancellationToken = default)
        {
            await _context.Products.AddAsync(products, cancellationToken);
        }

        public async Task AddProductImagesAsync(List<ProductPhotos> images, CancellationToken cancellationToken)
        {
            await _context.ProductPhotos.AddRangeAsync(images, cancellationToken);
        }

        public void Update(Products products)
        {
            _context.Products.Update(products);
        }

        public void Delete(Products products)
        {
            _context.Products.Remove(products);
        }

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
