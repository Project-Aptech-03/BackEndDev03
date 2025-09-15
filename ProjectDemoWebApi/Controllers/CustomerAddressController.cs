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
    public class CustomerAddressController : ControllerBase
    {
        private readonly ICustomerAddressService _customerAddressService;

        public CustomerAddressController(ICustomerAddressService customerAddressService)
        {
            _customerAddressService = customerAddressService;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User not authenticated");
        }

        /// <summary>
        /// Retrieves all addresses for the current customer (including the default address with IsDefault = true)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMyAddresses(CancellationToken cancellationToken = default)
        {
            var userId = GetUserId();
            var result = await _customerAddressService.GetUserAddressesAsync(userId, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Creates a new address for the current customer
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromBody] CreateCustomerAddressDto createAddressDto, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _customerAddressService.CreateAddressAsync(userId, createAddressDto, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Updates an address for the current customer
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] UpdateCustomerAddressDto updateAddressDto, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _customerAddressService.UpdateAddressAsync(userId, id, updateAddressDto, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Sets a default address for the current customer
        /// </summary>
        [HttpPut("{id}/set-default")]
        public async Task<IActionResult> SetDefaultAddress(int id, CancellationToken cancellationToken = default)
        {
            var userId = GetUserId();
            var result = await _customerAddressService.SetDefaultAddressAsync(userId, id, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Deletes an address for the current customer (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id, CancellationToken cancellationToken = default)
        {
            var userId = GetUserId();
            var result = await _customerAddressService.DeleteAddressAsync(userId, id, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }
    }
}