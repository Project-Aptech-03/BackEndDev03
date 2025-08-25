using AutoMapper;
using ProjectDemoWebApi.DTOs.ShoppingCart;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IProductsRepository _productsRepository;
        private readonly IMapper _mapper;

        public ShoppingCartService(
            IShoppingCartRepository shoppingCartRepository,
            IProductsRepository productsRepository,
            IMapper mapper)
        {
            _shoppingCartRepository = shoppingCartRepository ?? throw new ArgumentNullException(nameof(shoppingCartRepository));
            _productsRepository = productsRepository ?? throw new ArgumentNullException(nameof(productsRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ApiResponse<IEnumerable<ShoppingCartResponseDto>>> GetUserCartAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return ApiResponse<IEnumerable<ShoppingCartResponseDto>>.Fail(
                        "User ID cannot be empty.", 
                        null, 
                        400
                    );
                }

                var cartItems = await _shoppingCartRepository.GetUserCartAsync(userId, cancellationToken);
                var cartItemDtos = _mapper.Map<IEnumerable<ShoppingCartResponseDto>>(cartItems);
                
                return ApiResponse<IEnumerable<ShoppingCartResponseDto>>.Ok(
                    cartItemDtos, 
                    "Cart items retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ShoppingCartResponseDto>>.Fail(
                    "An error occurred while retrieving cart items.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<CartSummaryDto>> GetCartSummaryAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return ApiResponse<CartSummaryDto>.Fail(
                        "User ID cannot be empty.", 
                        null, 
                        400
                    );
                }

                var cartItems = await _shoppingCartRepository.GetUserCartAsync(userId, cancellationToken);
                var cartItemDtos = _mapper.Map<IEnumerable<ShoppingCartResponseDto>>(cartItems);
                
                var cartSummary = new CartSummaryDto
                {
                    Items = cartItemDtos.ToList(),
                    DeliveryCharges = CalculateDeliveryCharges(cartItemDtos.Sum(x => x.TotalPrice)),
                    DiscountAmount = 0 // This would be calculated based on applied coupons
                };
                
                return ApiResponse<CartSummaryDto>.Ok(
                    cartSummary, 
                    "Cart summary retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<CartSummaryDto>.Fail(
                    "An error occurred while retrieving cart summary.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<ShoppingCartResponseDto>> AddToCartAsync(string userId, AddToCartDto addToCartDto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return ApiResponse<ShoppingCartResponseDto>.Fail(
                        "User ID cannot be empty.", 
                        null, 
                        400
                    );
                }

                // Validate product exists and is active
                var product = await _productsRepository.GetByIdAsync(addToCartDto.ProductId, cancellationToken);
                if (product == null || !product.IsActive)
                {
                    return ApiResponse<ShoppingCartResponseDto>.Fail(
                        "Product not found or not available.", 
                        null, 
                        404
                    );
                }

                // Check stock availability
                if (product.StockQuantity < addToCartDto.Quantity)
                {
                    return ApiResponse<ShoppingCartResponseDto>.Fail(
                        "Insufficient stock available.", 
                        null, 
                        400
                    );
                }

                // Check if item already exists in cart
                var existingCartItem = await _shoppingCartRepository.GetUserCartItemAsync(userId, addToCartDto.ProductId, cancellationToken);
                
                if (existingCartItem != null)
                {
                    // Update quantity
                    var newQuantity = existingCartItem.Quantity + addToCartDto.Quantity;
                    
                    if (product.StockQuantity < newQuantity)
                    {
                        return ApiResponse<ShoppingCartResponseDto>.Fail(
                            "Insufficient stock available.", 
                            null, 
                            400
                        );
                    }
                    
                    existingCartItem.Quantity = newQuantity;
                    existingCartItem.UpdatedDate = DateTime.UtcNow;
                    
                    _shoppingCartRepository.Update(existingCartItem);
                    await _shoppingCartRepository.SaveChangesAsync(cancellationToken);
                    
                    var updatedCartItemDto = _mapper.Map<ShoppingCartResponseDto>(existingCartItem);
                    
                    return ApiResponse<ShoppingCartResponseDto>.Ok(
                        updatedCartItemDto, 
                        "Cart item updated successfully.", 
                        200
                    );
                }
                else
                {
                    // Add new item to cart
                    var cartItem = new ShoppingCart
                    {
                        UserId = userId,
                        ProductId = addToCartDto.ProductId,
                        Quantity = addToCartDto.Quantity,
                        UnitPrice = product.Price,
                        AddedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow
                    };

                    await _shoppingCartRepository.AddAsync(cartItem, cancellationToken);
                    await _shoppingCartRepository.SaveChangesAsync(cancellationToken);

                    var cartItemDto = _mapper.Map<ShoppingCartResponseDto>(cartItem);
                    
                    return ApiResponse<ShoppingCartResponseDto>.Ok(
                        cartItemDto, 
                        "Item added to cart successfully.", 
                        201
                    );
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<ShoppingCartResponseDto>.Fail(
                    "An error occurred while adding item to cart.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<ShoppingCartResponseDto?>> UpdateCartItemAsync(string userId, int productId, UpdateCartItemDto updateCartItemDto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return ApiResponse<ShoppingCartResponseDto?>.Fail(
                        "User ID cannot be empty.", 
                        null, 
                        400
                    );
                }

                var cartItem = await _shoppingCartRepository.GetUserCartItemAsync(userId, productId, cancellationToken);
                
                if (cartItem == null)
                {
                    return ApiResponse<ShoppingCartResponseDto?>.Fail(
                        "Cart item not found.", 
                        null, 
                        404
                    );
                }

                // Validate product stock
                var product = await _productsRepository.GetByIdAsync(productId, cancellationToken);
                if (product == null || !product.IsActive)
                {
                    return ApiResponse<ShoppingCartResponseDto?>.Fail(
                        "Product not found or not available.", 
                        null, 
                        404
                    );
                }

                if (product.StockQuantity < updateCartItemDto.Quantity)
                {
                    return ApiResponse<ShoppingCartResponseDto?>.Fail(
                        "Insufficient stock available.", 
                        null, 
                        400
                    );
                }

                cartItem.Quantity = updateCartItemDto.Quantity;
                cartItem.UpdatedDate = DateTime.UtcNow;

                _shoppingCartRepository.Update(cartItem);
                await _shoppingCartRepository.SaveChangesAsync(cancellationToken);

                var cartItemDto = _mapper.Map<ShoppingCartResponseDto>(cartItem);
                
                return ApiResponse<ShoppingCartResponseDto?>.Ok(
                    cartItemDto, 
                    "Cart item updated successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<ShoppingCartResponseDto?>.Fail(
                    "An error occurred while updating cart item.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<bool>> RemoveFromCartAsync(string userId, int productId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return ApiResponse<bool>.Fail(
                        "User ID cannot be empty.", 
                        false, 
                        400
                    );
                }

                var cartItem = await _shoppingCartRepository.GetUserCartItemAsync(userId, productId, cancellationToken);
                
                if (cartItem == null)
                {
                    return ApiResponse<bool>.Fail(
                        "Cart item not found.", 
                        false, 
                        404
                    );
                }

                _shoppingCartRepository.Delete(cartItem);
                await _shoppingCartRepository.SaveChangesAsync(cancellationToken);
                
                return ApiResponse<bool>.Ok(
                    true, 
                    "Item removed from cart successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(
                    "An error occurred while removing item from cart.", 
                    false, 
                    500
                );
            }
        }

        public async Task<ApiResponse<bool>> ClearCartAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return ApiResponse<bool>.Fail(
                        "User ID cannot be empty.", 
                        false, 
                        400
                    );
                }

                await _shoppingCartRepository.ClearUserCartAsync(userId, cancellationToken);
                await _shoppingCartRepository.SaveChangesAsync(cancellationToken);
                
                return ApiResponse<bool>.Ok(
                    true, 
                    "Cart cleared successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(
                    "An error occurred while clearing cart.", 
                    false, 
                    500
                );
            }
        }

        public async Task<ApiResponse<int>> GetCartItemCountAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return ApiResponse<int>.Fail(
                        "User ID cannot be empty.", 
                        0, 
                        400
                    );
                }

                var count = await _shoppingCartRepository.GetCartItemCountAsync(userId, cancellationToken);
                
                return ApiResponse<int>.Ok(
                    count, 
                    "Cart item count retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.Fail(
                    "An error occurred while retrieving cart item count.", 
                    0, 
                    500
                );
            }
        }

        public async Task<ApiResponse<decimal>> GetCartTotalAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return ApiResponse<decimal>.Fail(
                        "User ID cannot be empty.", 
                        0, 
                        400
                    );
                }

                var total = await _shoppingCartRepository.GetCartTotalAsync(userId, cancellationToken);
                
                return ApiResponse<decimal>.Ok(
                    total, 
                    "Cart total retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<decimal>.Fail(
                    "An error occurred while retrieving cart total.", 
                    0, 
                    500
                );
            }
        }

        private static decimal CalculateDeliveryCharges(decimal orderTotal)
        {
            // Simple delivery charge calculation - this could be more complex
            if (orderTotal >= 100) // Free delivery for orders over $100
                return 0;
            
            return 10; // Standard delivery charge
        }
    }
}