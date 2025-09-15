using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        [FromQuery] int pageSize = 10)
        {
            var result = await _publishersService.GetAllPublishersPageAsync(pageNumber, pageSize);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPublishers()
        {
            var result = await _publishersService.GetAllPublishersAsync();
            return StatusCode(result.StatusCode, result);
        }
    }
    
    
}
