using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectDemoWebApi.DTOs;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FaqController : ControllerBase
    {
        private readonly IFaqService _faqService;

        public FaqController(IFaqService faqService)
        {
            _faqService = faqService;
        }

        // GET: api/faq
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FaqDto>>> GetFaqs()
        {
            var faqs = await _faqService.GetActiveFaqsAsync();
            return Ok(faqs);
        }

        // GET: api/faq/all
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<FaqDto>>> GetAllFaqs()
        {
            var faqs = await _faqService.GetAllFaqsAsync();
            return Ok(faqs);
        }

        // GET: api/faq/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FaqDto>> GetFaq(int id)
        {
            var faq = await _faqService.GetFaqByIdAsync(id);

            if (faq == null)
            {
                return NotFound();
            }

            return Ok(faq);
        }

        // POST: api/faq
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<FaqDto>> PostFaq(CreateFaqDto createFaqDto)
        {
            var faq = await _faqService.CreateFaqAsync(createFaqDto);
            return CreatedAtAction(nameof(GetFaq), new { id = faq.Id }, faq);
        }

        // PUT: api/faq/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutFaq(int id, UpdateFaqDto updateFaqDto)
        {
            var result = await _faqService.UpdateFaqAsync(id, updateFaqDto);

            if (result == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/faq/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFaq(int id)
        {
            var result = await _faqService.DeleteFaqAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}