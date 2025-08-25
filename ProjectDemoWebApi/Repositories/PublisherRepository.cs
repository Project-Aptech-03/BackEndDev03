using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class PublisherRepository : BaseRepository<Publishers>, IPublisherRepository
    {
        public PublisherRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Publishers>> GetActivePublishersAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.PublisherName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Publishers>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(p => p.IsActive && p.PublisherName.Contains(searchTerm))
                .OrderBy(p => p.PublisherName)
                .ToListAsync(cancellationToken);
        }
    }
}