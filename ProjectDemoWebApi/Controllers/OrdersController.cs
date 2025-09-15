using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectDemoWebApi.DTOs.Order;
using ProjectDemoWebApi.Services.Interface;
using System.Security.Claims;

namespace ProjectDemoWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? throw new UnauthorizedAccessException("User not authenticated");
        }

        #region Admin Functions

        /// <summary>
        /// Get all orders - Admin only
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await _orderService.GetAllOrdersAsync();
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Update order status - Admin only
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int id, UpdateOrderDto updateOrderDto)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, updateOrderDto);
            return StatusCode(result.StatusCode, result);
        }

        #endregion

        #region Customer Functions

        /// <summary>
        /// Create order from shopping cart - Customer
        /// </summary>
        [HttpPost("from-cart")]
        public async Task<IActionResult> CreateOrderFromCart(CreateOrderFromCartDto createOrderFromCartDto)
        {
            var userId = GetUserId();
            var result = await _orderService.CreateOrderFromCartAsync(userId, createOrderFromCartDto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Cancel order - Customer (only if not shipped yet)
        /// </summary>
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id, [FromBody] CancelOrderDto cancelOrderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _orderService.CancelOrderAsync(id, userId, cancelOrderDto);
            return StatusCode(result.StatusCode, result);
        }

        #endregion

        #region Common Functions

        /// <summary>
        /// Get order by ID with full details
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var result = await _orderService.GetOrderByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get customer's own orders
        /// </summary>
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetUserId();
            var result = await _orderService.GetUserOrdersAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        #endregion
    }
}