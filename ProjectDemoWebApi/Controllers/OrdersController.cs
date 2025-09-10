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
        private readonly IPaymentVerificationService _paymentVerificationService;
        private readonly ISePayService _sePayService;

        public OrdersController(IOrderService orderService, IPaymentVerificationService paymentVerificationService, ISePayService sePayService)
        {
            _orderService = orderService;
            _paymentVerificationService = paymentVerificationService;
            _sePayService = sePayService;
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

        /// <summary>
        /// Get the 20 most recent transactions from SePay - Admin only
        /// </summary>
        [HttpGet("sepay-transactions")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSePayTransactions()
        {
            var result = await _sePayService.GetTransactionsAsync();
            return StatusCode(result.StatusCode, result);
        }

        #endregion

        #region Customer Functions

        /// <summary>
        /// Create an order and automatically start verification if it's a bank transfer
        /// </summary>
        [HttpPost("checkout")]
        public async Task<IActionResult> CheckoutOrder(CreateOrderDto createOrderDto)
        {
            var userId = GetUserId();

            // Create the order
            var orderResult = await _orderService.CreateOrderAsync(userId, createOrderDto);
            if (!orderResult.Success || orderResult.Data == null)
            {
                return StatusCode(orderResult.StatusCode, orderResult);
            }

            var order = orderResult.Data;

            // If it's a bank transfer, automatically start payment verification
            if (createOrderDto.PaymentType.Equals("Chuyển khoản", StringComparison.OrdinalIgnoreCase) ||
                createOrderDto.PaymentType.Equals("Bank Transfer", StringComparison.OrdinalIgnoreCase))
            {
                var transferContent = $"DH{order.OrderNumber}";

                // Start verification in the background (fire-and-forget)
                _ = Task.Run(async () =>
                {
                    await _paymentVerificationService.StartVerificationAsync(
                        order.Id,
                        order.TotalAmount,
                        transferContent
                    );
                });
            }

            return Ok(new
            {
                Success = true,
                Message = createOrderDto.PaymentType.Equals("Chuyển khoản", StringComparison.OrdinalIgnoreCase)
                    ? "Order created successfully. The system will automatically check for payment in the next 20 minutes."
                    : "Order created successfully.",
                Data = new
                {
                    Order = order,
                    TransferContent = createOrderDto.PaymentType.Equals("Chuyển khoản", StringComparison.OrdinalIgnoreCase)
                        ? $"DH{order.OrderNumber}"
                        : null,
                    AutoVerificationStarted = createOrderDto.PaymentType.Equals("Chuyển khoản", StringComparison.OrdinalIgnoreCase)
                },
                StatusCode = 200
            });
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

            // Stop verification if it's running
            _paymentVerificationService.StopVerification(id);

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

        /// <summary>
        /// Check the order's verification status (info only)
        /// </summary>
        [HttpGet("{id}/verification-status")]
        public IActionResult GetVerificationStatus(int id)
        {
            var isVerifying = _paymentVerificationService.IsVerifying(id);

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    OrderId = id,
                    IsVerifying = isVerifying,
                    Message = isVerifying ? "Automatically checking for payment" : "No verification currently running"
                },
                StatusCode = 200
            });
        }

        #endregion
    }
}
