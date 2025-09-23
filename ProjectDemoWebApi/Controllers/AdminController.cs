using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IProductsService _productsService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        public AdminController(
            IProductsService productsService,
            IOrderService orderService,
            IUserService userService)
        {
            _productsService = productsService;
            _orderService = orderService;
            _userService = userService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var allOrders = await _orderService.GetAllOrdersAsync();
            var lowStockProducts = await _productsService.GetLowStockProductsAsync(5);

            var totalSales = allOrders.Data?.Where(o => 
                string.Equals(o.OrderStatus, "Delivered", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(o.OrderStatus, "Completed", StringComparison.OrdinalIgnoreCase))
                .Sum(o => o.TotalAmount) ?? 0;

            var pendingOrdersCount = allOrders.Data?.Count(o => 
                string.Equals(o.OrderStatus, "Pending", StringComparison.OrdinalIgnoreCase)) ?? 0;

            var dashboard = new
            {
                TotalSales = totalSales,
                LowStockProductsCount = lowStockProducts.Data?.Count() ?? 0,
                PendingOrdersCount = pendingOrdersCount,
                TotalOrdersCount = allOrders.Data?.Count() ?? 0,
                Date = DateTime.UtcNow
            };

            return Ok(new { Success = true, Data = dashboard });
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(new { Success = true, Data = users });
        }

        [HttpGet("sales/monthly")]
        public async Task<IActionResult> GetMonthlySales([FromQuery] int year = 0)
        {
            if (year == 0) year = DateTime.UtcNow.Year;

            var allOrders = await _orderService.GetAllOrdersAsync();
            if (!allOrders.Success || allOrders.Data == null)
            {
                return Ok(new { Success = false, Message = "Unable to retrieve orders data" });
            }

            var completedOrders = allOrders.Data.Where(o => 
                string.Equals(o.OrderStatus, "Delivered", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(o.OrderStatus, "Completed", StringComparison.OrdinalIgnoreCase));

            var monthlySales = new List<object>();
            
            for (int month = 1; month <= 12; month++)
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                
                var sales = completedOrders
                    .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                    .Sum(o => o.TotalAmount);
                
                monthlySales.Add(new
                {
                    Month = month,
                    MonthName = startDate.ToString("MMMM"),
                    Sales = sales
                });
            }

            return Ok(new { Success = true, Data = monthlySales });
        }

        [HttpGet("products/low-stock")]
        public async Task<IActionResult> GetLowStockProducts([FromQuery] int threshold = 10)
        {
            var result = await _productsService.GetLowStockProductsAsync(threshold);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("orders/pending")]
        public async Task<IActionResult> GetPendingOrders()
        {
            var allOrders = await _orderService.GetAllOrdersAsync();
            if (!allOrders.Success || allOrders.Data == null)
            {
                return StatusCode(allOrders.StatusCode, allOrders);
            }

            var pendingOrders = allOrders.Data.Where(o => 
                string.Equals(o.OrderStatus, "Pending", StringComparison.OrdinalIgnoreCase));

            return Ok(new 
            { 
                Success = true, 
                Message = "Pending orders retrieved successfully.",
                Data = pendingOrders,
                StatusCode = 200 
            });
        }
    }
}