using ProjectDemoWebApi.DTOs.Order;
using ProjectDemoWebApi.DTOs.Shared;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IOrderService
    {
        // Admin functions
        Task<ApiResponse<IEnumerable<OrderResponseDto>>> GetAllOrdersAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<OrderResponseDto?>> UpdateOrderStatusAsync(int id, UpdateOrderDto updateOrderDto, CancellationToken cancellationToken = default);
        
        // Customer functions
        Task<ApiResponse<OrderResponseDto>> CreateOrderAsync(string userId, CreateOrderDto createOrderDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> CancelOrderAsync(int id, string userId, CancelOrderDto cancelOrderDto, CancellationToken cancellationToken = default);
        
        // Common functions
        Task<ApiResponse<OrderResponseDto?>> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ApiResponse<IEnumerable<OrderResponseDto>>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default);
        
        // Order code generation
        Task<ApiResponse<string>> GetNextOrderCodeAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<IEnumerable<BestSellerProductDto>>> GetTop3ProductsAsync(CancellationToken cancellationToken = default);

    }
}