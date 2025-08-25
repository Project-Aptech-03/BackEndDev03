using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectDemoWebApi.DTOs.CustomerAddress;
using ProjectDemoWebApi.Services.Interface;
using System.Security.Claims;

namespace ProjectDemoWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly ICustomerAddressService _addressService;

        public AddressesController(ICustomerAddressService addressService)
        {
            _addressService = addressService;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? throw new UnauthorizedAccessException("User not authenticated");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyAddresses()
        {
            var userId = GetUserId();
            var result = await _addressService.GetUserAddressesAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("default")]
        public async Task<IActionResult> GetDefaultAddress()
        {
            var userId = GetUserId();
            var result = await _addressService.GetUserDefaultAddressAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddress(int id)
        {
            var result = await _addressService.GetAddressByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress(CreateCustomerAddressDto createAddressDto)
        {
            var userId = GetUserId();
            var result = await _addressService.CreateAddressAsync(userId, createAddressDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(int id, UpdateCustomerAddressDto updateAddressDto)
        {
            var userId = GetUserId();
            var result = await _addressService.UpdateAddressAsync(userId, id, updateAddressDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{id}/set-default")]
        public async Task<IActionResult> SetDefaultAddress(int id)
        {
            var userId = GetUserId();
            var result = await _addressService.SetDefaultAddressAsync(userId, id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var userId = GetUserId();
            var result = await _addressService.DeleteAddressAsync(userId, id);
            return StatusCode(result.StatusCode, result);
        }
    }
}