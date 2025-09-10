using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories.Implementations
{
    public class FaqRepository : IFaqRepository
    {
        private readonly ApplicationDbContext _context;

        public FaqRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Faq>> GetAllFaqsAsync()
        {
            return await _context.Faqs
                .OrderBy(f => f.DisplayOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<Faq>> GetActiveFaqsAsync()
        {
            return await _context.Faqs
                .Where(f => f.IsActive)
                .OrderBy(f => f.DisplayOrder)
                .ToListAsync();
        }

        public async Task<Faq> GetFaqByIdAsync(int id)
        {
            return await _context.Faqs.FindAsync(id);
        }

        public async Task<Faq> CreateFaqAsync(Faq faq)
        {
            _context.Faqs.Add(faq);
            await _context.SaveChangesAsync();
            return faq;
        }

        public async Task<Faq> UpdateFaqAsync(Faq faq)
        {
            _context.Faqs.Update(faq);
            await _context.SaveChangesAsync();
            return faq;
        }

        public async Task<bool> DeleteFaqAsync(int id)
        {
            var faq = await _context.Faqs.FindAsync(id);
            if (faq == null)
                return false;

            _context.Faqs.Remove(faq);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}