using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IFaqRepository
    {
        Task<IEnumerable<FAQ>> GetAllFaqsAsync();
        Task<IEnumerable<FAQ>> GetActiveFaqsAsync();
        Task<FAQ> GetFaqByIdAsync(int id);
        Task<FAQ> CreateFaqAsync(FAQ faq);
        Task<FAQ> UpdateFaqAsync(FAQ faq);
        Task<bool> DeleteFaqAsync(int id);
    }
}
