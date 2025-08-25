using ProjectDemoWebApi.DTOs.Order;
using ProjectDemoWebApi.DTOs.Shared;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IOrderService
    {
        Task<ApiResponse<IEnumerable<OrderResponseDto>>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default);
        Task<ApiResponse<OrderResponseDto?>> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ApiResponse<OrderResponseDto?>> GetOrderByNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
        Task<ApiResponse<IEnumerable<OrderResponseDto>>> GetOrdersByStatusAsync(string status, CancellationToken cancellationToken = default);
        Task<ApiResponse<OrderResponseDto>> CreateOrderAsync(string userId, CreateOrderDto createOrderDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<OrderResponseDto?>> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> CancelOrderAsync(int id, string userId, CancellationToken cancellationToken = default);
        Task<ApiResponse<decimal>> GetTotalSalesAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
        Task<ApiResponse<IEnumerable<OrderResponseDto>>> GetPendingOrdersAsync(CancellationToken cancellationToken = default);
    }
}