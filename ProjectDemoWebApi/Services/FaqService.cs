using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ProjectDemoWebApi.DTOs;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Services.Implementations
{
    public class FaqService : IFaqService
    {
        private readonly IFaqRepository _faqRepository;
        private readonly IMapper _mapper;

        public FaqService(IFaqRepository faqRepository, IMapper mapper)
        {
            _faqRepository = faqRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FaqDto>> GetAllFaqsAsync()
        {
            var faqs = await _faqRepository.GetAllFaqsAsync();
            return _mapper.Map<IEnumerable<FaqDto>>(faqs);
        }

        public async Task<IEnumerable<FaqDto>> GetActiveFaqsAsync()
        {
            var faqs = await _faqRepository.GetActiveFaqsAsync();
            return _mapper.Map<IEnumerable<FaqDto>>(faqs);
        }

        public async Task<FaqDto> GetFaqByIdAsync(int id)
        {
            var faq = await _faqRepository.GetFaqByIdAsync(id);
            return _mapper.Map<FaqDto>(faq);
        }

        public async Task<FaqDto> CreateFaqAsync(CreateFaqDto createFaqDto)
        {
            var faq = _mapper.Map<Faq>(createFaqDto);
            var createdFaq = await _faqRepository.CreateFaqAsync(faq);
            return _mapper.Map<FaqDto>(createdFaq);
        }

        public async Task<FaqDto> UpdateFaqAsync(int id, UpdateFaqDto updateFaqDto)
        {
            var existingFaq = await _faqRepository.GetFaqByIdAsync(id);
            if (existingFaq == null)
                return null;

            _mapper.Map(updateFaqDto, existingFaq);
            existingFaq.UpdatedAt = System.DateTime.UtcNow;

            var updatedFaq = await _faqRepository.UpdateFaqAsync(existingFaq);
            return _mapper.Map<FaqDto>(updatedFaq);
        }

        public async Task<bool> DeleteFaqAsync(int id)
        {
            return await _faqRepository.DeleteFaqAsync(id);
        }
    }
}