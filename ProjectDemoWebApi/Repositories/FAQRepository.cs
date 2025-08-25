using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class FAQRepository : BaseRepository<FAQ>, IFAQRepository
    {
        public FAQRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<FAQ>> GetActiveFAQsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(f => f.IsActive)
                .OrderBy(f => f.SortOrder)
                .ThenBy(f => f.Question)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<FAQ>> GetFAQsByOrderAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .OrderBy(f => f.SortOrder)
                .ThenBy(f => f.Question)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<FAQ>> SearchFAQsAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(f => f.IsActive && 
                           (f.Question.Contains(searchTerm) || f.Answer.Contains(searchTerm)))
                .OrderBy(f => f.SortOrder)
                .ThenBy(f => f.Question)
                .ToListAsync(cancellationToken);
        }

        public async Task ReorderFAQsAsync(List<(int Id, int SortOrder)> faqOrders, CancellationToken cancellationToken = default)
        {
            var faqIds = faqOrders.Select(x => x.Id).ToList();
            var faqs = await _dbSet
                .Where(f => faqIds.Contains(f.Id))
                .ToListAsync(cancellationToken);

            foreach (var faq in faqs)
            {
                var orderItem = faqOrders.FirstOrDefault(x => x.Id == faq.Id);
                if (orderItem != default)
                {
                    faq.SortOrder = orderItem.SortOrder;
                }
            }

            _dbSet.UpdateRange(faqs);
        }
    }
}