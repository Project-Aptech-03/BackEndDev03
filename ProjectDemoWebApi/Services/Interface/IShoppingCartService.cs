using ProjectDemoWebApi.DTOs.ShoppingCart;
using ProjectDemoWebApi.DTOs.Shared;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IShoppingCartService
    {
        Task<ApiResponse<IEnumerable<ShoppingCartResponseDto>>> GetUserCartAsync(string userId, CancellationToken cancellationToken = default);
        Task<ApiResponse<CartSummaryDto>> GetCartSummaryAsync(string userId, CancellationToken cancellationToken = default);
        Task<ApiResponse<ShoppingCartResponseDto>> AddToCartAsync(string userId, AddToCartDto addToCartDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<ShoppingCartResponseDto?>> UpdateCartItemAsync(string userId, int cartId, UpdateCartItemDto updateCartItemDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> RemoveFromCartAsync(string userId, int productId, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> ClearCartAsync(string userId, CancellationToken cancellationToken = default);
        Task<ApiResponse<int>> GetCartItemCountAsync(string userId, CancellationToken cancellationToken = default);
        Task<ApiResponse<decimal>> GetCartTotalAsync(string userId, CancellationToken cancellationToken = default);
    }
}