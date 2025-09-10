using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IFaqRepository
    {
        Task<IEnumerable<Faq>> GetAllFaqsAsync();
        Task<IEnumerable<Faq>> GetActiveFaqsAsync();
        Task<Faq> GetFaqByIdAsync(int id);
        Task<Faq> CreateFaqAsync(Faq faq);
        Task<Faq> UpdateFaqAsync(Faq faq);
        Task<bool> DeleteFaqAsync(int id);
    }
}