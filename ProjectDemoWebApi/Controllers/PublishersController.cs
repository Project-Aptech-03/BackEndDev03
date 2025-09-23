using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectDemoWebApi.DTOs.Publisher;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly IPublishersService _publishersService;

        public PublishersController(IPublishersService publishersService) {
            _publishersService = publishersService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPublishersPaged(
         [FromQuery] int pageNumber = 1,
         [FromQuery] int pageSize = 10,
         [FromQuery] string? keyword = null) 
        {
            var result = await _publishersService.GetAllPublishersPageAsync(pageNumber, pageSize, keyword);
            return StatusCode(result.StatusCode, result);
        }



        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPublishers()
        {
            var result = await _publishersService.GetAllPublishersAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> CreatePublisher([FromBody] CreatePublisherDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _publishersService.CreatePublisherAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Employee")]

        public async Task<IActionResult> UpdatePublisher(int id, [FromBody] UpdatePublisherDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _publishersService.UpdatePublisherAsync(id, dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Employee")]

        public async Task<IActionResult> DeletePublisher(int id)
        {
            var result = await _publishersService.DeletePublisherAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
    

