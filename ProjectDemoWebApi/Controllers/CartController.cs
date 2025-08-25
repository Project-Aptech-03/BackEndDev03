using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectDemoWebApi.DTOs.ShoppingCart;
using ProjectDemoWebApi.Services.Interface;
using System.Security.Claims;

namespace ProjectDemoWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IShoppingCartService _cartService;

        public CartController(IShoppingCartService cartService)
        {
            _cartService = cartService;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? throw new UnauthorizedAccessException("User not authenticated");
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            var result = await _cartService.GetUserCartAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetCartSummary()
        {
            var userId = GetUserId();
            var result = await _cartService.GetCartSummaryAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCartItemCount()
        {
            var userId = GetUserId();
            var result = await _cartService.GetCartItemCountAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("total")]
        public async Task<IActionResult> GetCartTotal()
        {
            var userId = GetUserId();
            var result = await _cartService.GetCartTotalAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(AddToCartDto addToCartDto)
        {
            var userId = GetUserId();
            var result = await _cartService.AddToCartAsync(userId, addToCartDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateCartItem(int productId, UpdateCartItemDto updateCartItemDto)
        {
            var userId = GetUserId();
            var result = await _cartService.UpdateCartItemAsync(userId, productId, updateCartItemDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = GetUserId();
            var result = await _cartService.RemoveFromCartAsync(userId, productId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            var result = await _cartService.ClearCartAsync(userId);
            return StatusCode(result.StatusCode, result);
        }
    }
}