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
            // Get basic dashboard data
            var totalSales = await _orderService.GetTotalSalesAsync();
            var lowStockProducts = await _productsService.GetLowStockProductsAsync(5);
            var pendingOrders = await _orderService.GetPendingOrdersAsync();

            var dashboard = new
            {
                TotalSales = totalSales.Data,
                LowStockProductsCount = lowStockProducts.Data?.Count() ?? 0,
                PendingOrdersCount = pendingOrders.Data?.Count() ?? 0,
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

            var monthlySales = new List<object>();
            
            for (int month = 1; month <= 12; month++)
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                
                var sales = await _orderService.GetTotalSalesAsync(startDate, endDate);
                
                monthlySales.Add(new
                {
                    Month = month,
                    MonthName = startDate.ToString("MMMM"),
                    Sales = sales.Data
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
            var result = await _orderService.GetPendingOrdersAsync();
            return StatusCode(result.StatusCode, result);
        }
    }
}