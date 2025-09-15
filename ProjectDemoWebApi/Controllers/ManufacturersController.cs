using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectDemoWebApi.DTOs.Manufacturer;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ManufacturersController : ControllerBase
    {
        private readonly IManufacturerService _manufacturerService;

        public ManufacturersController(IManufacturerService manufacturerService)
        {
            _manufacturerService = manufacturerService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllManufacturersPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? keyword = null)
        {
            var result = await _manufacturerService.GetAllManufacturersPageAsync(pageNumber, pageSize, keyword);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetManufacturers()
        {
            var result = await _manufacturerService.GetActiveManufacturersAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetManufacturer(int id)
        {
            var result = await _manufacturerService.GetManufacturerByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("code/{code}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetManufacturerByCode(string code)
        {
            var result = await _manufacturerService.GetManufacturerByCodeAsync(code);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateManufacturer(CreateManufacturerDto createManufacturerDto)
        {
            var result = await _manufacturerService.CreateManufacturerAsync(createManufacturerDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateManufacturer(int id, UpdateManufacturerDto updateManufacturerDto)
        {
            var result = await _manufacturerService.UpdateManufacturerAsync(id, updateManufacturerDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteManufacturer(int id)
        {
            var result = await _manufacturerService.DeleteManufacturerAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}