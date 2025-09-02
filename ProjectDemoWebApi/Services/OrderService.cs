using AutoMapper;
using ProjectDemoWebApi.DTOs.Order;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IProductsRepository _productsRepository;
        private readonly ICustomerAddressRepository _customerAddressRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IStockMovementRepository _stockMovementRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IProductsRepository productsRepository,
            ICustomerAddressRepository customerAddressRepository,
            IShoppingCartRepository shoppingCartRepository,
            IStockMovementRepository stockMovementRepository,
            ICouponRepository couponRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _orderItemRepository = orderItemRepository ?? throw new ArgumentNullException(nameof(orderItemRepository));
            _productsRepository = productsRepository ?? throw new ArgumentNullException(nameof(productsRepository));
            _customerAddressRepository = customerAddressRepository ?? throw new ArgumentNullException(nameof(customerAddressRepository));
            _shoppingCartRepository = shoppingCartRepository ?? throw new ArgumentNullException(nameof(shoppingCartRepository));
            _stockMovementRepository = stockMovementRepository ?? throw new ArgumentNullException(nameof(stockMovementRepository));
            _couponRepository = couponRepository ?? throw new ArgumentNullException(nameof(couponRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #region Admin Functions

        public async Task<ApiResponse<IEnumerable<OrderResponseDto>>> GetAllOrdersAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var orders = await _orderRepository.GetAllOrdersWithDetailsAsync(cancellationToken);
                var orderDtos = _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
                
                return ApiResponse<IEnumerable<OrderResponseDto>>.Ok(
                    orderDtos, 
                    "All orders retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<OrderResponseDto>>.Fail(
                    "An error occurred while retrieving all orders.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<OrderResponseDto?>> UpdateOrderStatusAsync(int id, UpdateOrderDto updateOrderDto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return ApiResponse<OrderResponseDto?>.Fail(
                        "Invalid order ID.", 
                        null, 
                        400
                    );
                }

                var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
                
                if (order == null)
                {
                    return ApiResponse<OrderResponseDto?>.Fail(
                        "Order not found.", 
                        null, 
                        404
                    );
                }

                // Update order status
                if (!string.IsNullOrWhiteSpace(updateOrderDto.OrderStatus))
                    order.OrderStatus = updateOrderDto.OrderStatus;
                
                if (!string.IsNullOrWhiteSpace(updateOrderDto.PaymentStatus))
                    order.PaymentStatus = updateOrderDto.PaymentStatus;

                order.UpdatedDate = DateTime.UtcNow;

                _orderRepository.Update(order);
                await _orderRepository.SaveChangesAsync(cancellationToken);

                // Get updated order with details for response
                var updatedOrder = await _orderRepository.GetByIdWithDetailsAsync(order.Id, cancellationToken);
                var orderDto = _mapper.Map<OrderResponseDto>(updatedOrder);
                
                return ApiResponse<OrderResponseDto?>.Ok(
                    orderDto, 
                    "Order status updated successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<OrderResponseDto?>.Fail(
                    "An error occurred while updating the order status.", 
                    null, 
                    500
                );
            }
        }

        #endregion

        #region Customer Functions

        public async Task<ApiResponse<OrderResponseDto>> CreateOrderFromCartAsync(string userId, CreateOrderFromCartDto createOrderFromCartDto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return ApiResponse<OrderResponseDto>.Fail(
                        "User ID cannot be empty.", 
                        null, 
                        400
                    );
                }

                // Validate delivery address
                var address = await _customerAddressRepository.GetByIdAsync(createOrderFromCartDto.DeliveryAddressId, cancellationToken);
                if (address == null || address.UserId != userId || !address.IsActive)
                {
                    return ApiResponse<OrderResponseDto>.Fail(
                        "Invalid delivery address.", 
                        null, 
                        400
                    );
                }

                // Get cart items
                var cartItems = await _shoppingCartRepository.GetUserCartAsync(userId, cancellationToken);
                
                if (!cartItems.Any())
                {
                    return ApiResponse<OrderResponseDto>.Fail(
                        "Shopping cart is empty.", 
                        null, 
                        400
                    );
                }

                // Filter cart items if specific ID
                var cartItemsToOrder = cartItems.AsEnumerable();
                if (createOrderFromCartDto.CartItemIds != null && createOrderFromCartDto.CartItemIds.Any())
                {
                    cartItemsToOrder = cartItems.Where(ci => createOrderFromCartDto.CartItemIds.Contains(ci.Id));
                    
                    if (!cartItemsToOrder.Any())
                    {
                        return ApiResponse<OrderResponseDto>.Fail(
                            "No valid cart items found for the specified IDs.", 
                            null, 
                            400
                        );
                    }
                }

                // Validate cart items and calculate totals
                decimal subtotal = 0;
                var orderItems = new List<OrderItems>();

                foreach (var cartItem in cartItemsToOrder)
                {
                    var product = cartItem.Product ?? await _productsRepository.GetByIdNoTrackingAsync(cartItem.ProductId, cancellationToken);
                    if (product == null || !product.IsActive)
                    {
                        return ApiResponse<OrderResponseDto>.Fail(
                            $"Product with ID {cartItem.ProductId} not found or not available.", 
                            null, 
                            400
                        );
                    }

                    if (product.StockQuantity < cartItem.Quantity)
                    {
                        return ApiResponse<OrderResponseDto>.Fail(
                            $"Insufficient stock for product {product.ProductName}. Available: {product.StockQuantity}, Requested: {cartItem.Quantity}", 
                            null, 
                            400
                        );
                    }

                    var orderItem = new OrderItems
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.UnitPrice,
                        DiscountPercent = 0,
                        DiscountAmount = 0,
                        TotalPrice = cartItem.UnitPrice * cartItem.Quantity,
                        Notes = null
                    };

                    orderItems.Add(orderItem);
                    subtotal += orderItem.TotalPrice;
                }

                // Apply coupons if provided
                decimal couponDiscountAmount = 0;
                var appliedCoupons = new List<string>();

                if (createOrderFromCartDto.CouponCodes != null && createOrderFromCartDto.CouponCodes.Any())
                {
                    foreach (var couponCode in createOrderFromCartDto.CouponCodes)
                    {
                        var coupon = await _couponRepository.GetByCouponCodeAsync(couponCode, cancellationToken);
                        if (coupon != null && await _couponRepository.IsCouponValidAsync(couponCode, subtotal, cancellationToken))
                        {
                            decimal discount = 0;
                            if (string.Equals(coupon.DiscountType, "percentage", StringComparison.OrdinalIgnoreCase))
                            {
                                discount = subtotal * (coupon.DiscountValue / 100);
                                if (coupon.MaxDiscountAmount > 0 && discount > coupon.MaxDiscountAmount)
                                    discount = coupon.MaxDiscountAmount;
                            }
                            else
                            {
                                discount = coupon.DiscountValue;
                            }

                            couponDiscountAmount += discount;
                            appliedCoupons.Add(couponCode);

                            // Decrement coupon quantity
                            await _couponRepository.DecrementCouponQuantityAsync(coupon.Id, cancellationToken);
                        }
                    }
                }

                // Calculate delivery charges
                decimal deliveryCharges = CalculateDeliveryCharges(subtotal, address.DistanceKm);

                // Create order
                var order = new Orders
                {
                    OrderNumber = await _orderRepository.GenerateOrderNumberAsync(cancellationToken),
                    CustomerId = userId,
                    DeliveryAddressId = createOrderFromCartDto.DeliveryAddressId,
                    OrderDate = DateTime.UtcNow,
                    Subtotal = subtotal,
                    CouponDiscountAmount = couponDiscountAmount,
                    DeliveryCharges = deliveryCharges,
                    TotalAmount = subtotal - couponDiscountAmount + deliveryCharges,
                    OrderStatus = "Pending",
                    PaymentType = createOrderFromCartDto.PaymentType,
                    PaymentStatus = "Pending",
                    AppliedCoupons = appliedCoupons.Any() ? string.Join(",", appliedCoupons) : null,
                    DeliveryNotes = createOrderFromCartDto.DeliveryNotes,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                await _orderRepository.AddAsync(order, cancellationToken);
                await _orderRepository.SaveChangesAsync(cancellationToken);

                // Add order items
                foreach (var orderItem in orderItems)
                {
                    orderItem.OrderId = order.Id;
                }

                await _orderItemRepository.AddRangeAsync(orderItems, cancellationToken);
                await _orderItemRepository.SaveChangesAsync(cancellationToken);

                // Update product stock and add stock movements
                foreach (var orderItem in orderItems)
                {
                    var product = await _productsRepository.GetByIdNoTrackingAsync(orderItem.ProductId, cancellationToken);
                    if (product != null)
                    {
                        var previousStock = product.StockQuantity;
                        var newStock = previousStock - orderItem.Quantity;
                        
                        await _productsRepository.UpdateStockAsync(orderItem.ProductId, newStock, cancellationToken);
                        
                        await _stockMovementRepository.AddStockMovementAsync(
                            orderItem.ProductId,
                            -orderItem.Quantity,
                            (int)previousStock,
                            (int)newStock,
                            "SALE",
                            order.Id,
                            orderItem.UnitPrice,
                            $"Order {order.OrderNumber}",
                            userId,
                            cancellationToken);
                    }
                }

                await _productsRepository.SaveChangesAsync(cancellationToken);
                await _stockMovementRepository.SaveChangesAsync(cancellationToken);

                // Remove only the ordered items from cart (keep remaining items)
                foreach (var cartItem in cartItemsToOrder)
                {
                    _shoppingCartRepository.Delete(cartItem);
                }
                await _shoppingCartRepository.SaveChangesAsync(cancellationToken);

                // Get the complete order for response
                var createdOrder = await _orderRepository.GetByIdWithDetailsAsync(order.Id, cancellationToken);
                var orderDto = _mapper.Map<OrderResponseDto>(createdOrder);
                
                return ApiResponse<OrderResponseDto>.Ok(
                    orderDto, 
                    "Order created successfully from shopping cart.", 
                    201
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<OrderResponseDto>.Fail(
                    "An error occurred while creating the order from cart.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<bool>> CancelOrderAsync(int id, string userId, CancelOrderDto cancelOrderDto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return ApiResponse<bool>.Fail(
                        "Invalid order ID.", 
                        false, 
                        400
                    );
                }

                if (cancelOrderDto == null)
                {
                    return ApiResponse<bool>.Fail(
                        "Cancellation reason is required.", 
                        false, 
                        400
                    );
                }

                var order = await _orderRepository.GetByIdWithDetailsAsync(id, cancellationToken);
                
                if (order == null)
                {
                    return ApiResponse<bool>.Fail(
                        "Order not found.", 
                        false, 
                        404
                    );
                }

                if (order.CustomerId != userId)
                {
                    return ApiResponse<bool>.Fail(
                        "You can only cancel your own orders.", 
                        false, 
                        403
                    );
                }

                // Check if order status allows cancellation - only allow cancellation for Pending, Confirmed, Processing
                var cancellableStatuses = new[] { "Pending", "Confirmed", "Processing" };
                if (!cancellableStatuses.Any(status => string.Equals(status, order.OrderStatus, StringComparison.OrdinalIgnoreCase)))
                {
                    return ApiResponse<bool>.Fail(
                        $"Order cannot be cancelled. Current status: {order.OrderStatus}. Orders can only be cancelled when status is Pending, Confirmed, or Processing.", 
                        false, 
                        400
                    );
                }

                order.OrderStatus = "Cancelled";
                order.CancellationReason = cancelOrderDto.CancellationReason;
                order.CancelledDate = DateTime.UtcNow;
                order.UpdatedDate = DateTime.UtcNow;

                // Restore stock for cancelled order
                foreach (var orderItem in order.OrderItems)
                {
                    var product = await _productsRepository.GetByIdNoTrackingAsync(orderItem.ProductId, cancellationToken);
                    if (product != null)
                    {
                        var previousStock = product.StockQuantity;
                        var newStock = previousStock + orderItem.Quantity;
                        
                        await _productsRepository.UpdateStockAsync(orderItem.ProductId, newStock, cancellationToken);
                        
                        await _stockMovementRepository.AddStockMovementAsync(
                            orderItem.ProductId,
                            orderItem.Quantity,
                            (int)previousStock,
                            (int)newStock,
                            "CANCELLATION",
                            order.Id,
                            orderItem.UnitPrice,
                            $"Order {order.OrderNumber} cancelled - Reason: {cancelOrderDto.CancellationReason}",
                            userId,
                            cancellationToken);
                    }
                }

                _orderRepository.Update(order);
                await _orderRepository.SaveChangesAsync(cancellationToken);
                await _productsRepository.SaveChangesAsync(cancellationToken);
                await _stockMovementRepository.SaveChangesAsync(cancellationToken);
                
                return ApiResponse<bool>.Ok(
                    true, 
                    "Order cancelled successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(
                    "An error occurred while cancelling the order.", 
                    false, 
                    500
                );
            }
        }

        #endregion

        #region Common Functions

        public async Task<ApiResponse<OrderResponseDto?>> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return ApiResponse<OrderResponseDto?>.Fail(
                        "Invalid order ID.", 
                        null, 
                        400
                    );
                }

                var order = await _orderRepository.GetByIdWithDetailsAsync(id, cancellationToken);
                
                if (order == null)
                {
                    return ApiResponse<OrderResponseDto?>.Fail(
                        "Order not found.", 
                        null, 
                        404
                    );
                }

                var orderDto = _mapper.Map<OrderResponseDto>(order);
                
                return ApiResponse<OrderResponseDto?>.Ok(
                    orderDto, 
                    "Order retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<OrderResponseDto?>.Fail(
                    "An error occurred while retrieving the order.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<OrderResponseDto>>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return ApiResponse<IEnumerable<OrderResponseDto>>.Fail(
                        "User ID cannot be empty.", 
                        null, 
                        400
                    );
                }

                var orders = await _orderRepository.GetUserOrdersAsync(userId, cancellationToken);
                var orderDtos = _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
                
                return ApiResponse<IEnumerable<OrderResponseDto>>.Ok(
                    orderDtos, 
                    "Orders retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<OrderResponseDto>>.Fail(
                    "An error occurred while retrieving orders.", 
                    null, 
                    500
                );
            }
        }

        #endregion

        #region Private Methods

        private static decimal CalculateDeliveryCharges(decimal orderTotal, decimal? distanceKm)
        {
            // Free delivery for orders over 100,000 VND
            if (orderTotal >= 100000)
                return 0;

            // Base delivery charge
            decimal baseCharge = 25000; // 25,000 VND

            // Additional charge based on distance
            if (distanceKm.HasValue && distanceKm > 10)
            {
                baseCharge += (distanceKm.Value - 10) * 5000; // 5,000 VND per km over 10km
            }

            return Math.Min(baseCharge, 100000); // Cap at 100,000 VND
        }

        #endregion
    }
}