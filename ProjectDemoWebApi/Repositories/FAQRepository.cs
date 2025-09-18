using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class FaqRepository : IFaqRepository
    {
        private readonly ApplicationDbContext _context;

        public FaqRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FAQ>> GetAllFaqsAsync()
        {
            return await _context.FAQ
                .OrderBy(f => f.DisplayOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<FAQ>> GetActiveFaqsAsync()
        {
            return await _context.FAQ
                .Where(f => f.IsActive)
                .OrderBy(f => f.DisplayOrder)
                .ToListAsync();
        }

        public async Task<FAQ> GetFaqByIdAsync(int id)
        {
            return await _context.FAQ.FindAsync(id);
        }

        public async Task<FAQ> CreateFaqAsync(FAQ faq)
        {
            _context.FAQ.Add(faq);
            await _context.SaveChangesAsync();
            return faq;
        }

        public async Task<FAQ> UpdateFaqAsync(FAQ faq)
        {
            _context.FAQ.Update(faq);
            await _context.SaveChangesAsync();
            return faq;
        }

        public async Task<bool> DeleteFaqAsync(int id)
        {
            var faq = await _context.FAQ.FindAsync(id);
            if (faq == null)
                return false;

            _context.FAQ.Remove(faq);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}