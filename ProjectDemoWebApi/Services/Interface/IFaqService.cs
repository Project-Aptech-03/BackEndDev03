using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectDemoWebApi.DTOs;
using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IFaqService
    {
        Task<IEnumerable<FaqDto>> GetAllFaqsAsync();
        Task<IEnumerable<FaqDto>> GetActiveFaqsAsync();
        Task<FaqDto> GetFaqByIdAsync(int id);
        Task<FaqDto> CreateFaqAsync(CreateFaqDto createFaqDto);
        Task<FaqDto> UpdateFaqAsync(int id, UpdateFaqDto updateFaqDto);
        Task<bool> DeleteFaqAsync(int id);
    }
}