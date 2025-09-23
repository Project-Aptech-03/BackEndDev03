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

        // Top 3 sản phẩm bán chạy nhất
        [HttpGet("top-products")]
        public async Task<IActionResult> GetTop3Products(CancellationToken cancellationToken)
        {
            var response = await _orderService.GetTop3ProductsAsync(cancellationToken);
            if (!response.Success) return StatusCode(response.StatusCode, response);
            return Ok(response);
        }
        #region Admin Functions

        /// <summary>
        /// Get all orders - Admin only
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await _orderService.GetAllOrdersAsync();
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Update order status - Admin only
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> UpdateOrderStatus(int id, UpdateOrderDto updateOrderDto)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, updateOrderDto);
            return StatusCode(result.StatusCode, result);
        }

        #endregion

        #region Customer Functions

        /// <summary>
        /// Create an order
        /// </summary>
        [HttpPost("checkout")]
        public async Task<IActionResult> CheckoutOrder(CreateOrderDto createOrderDto)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            // Create the order
            var orderResult = await _orderService.CreateOrderAsync(userId, createOrderDto);

            return StatusCode(orderResult.StatusCode, orderResult);
                
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

        #region Order Code Generation

        /// <summary>
        /// Get the next available order code for checkout preparation
        /// </summary>
        [HttpGet("next-order-code")]
        public async Task<IActionResult> GetNextOrderCode()
        {
            var result = await _orderService.GetNextOrderCodeAsync();
            return StatusCode(result.StatusCode, result);
        }

        #endregion

        #region Common Functions

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

        /// <summary>
        /// Get order by ID with full details
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var result = await _orderService.GetOrderByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        #endregion
    }
}
