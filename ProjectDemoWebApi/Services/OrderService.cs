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

        public async Task<ApiResponse<OrderResponseDto?>> GetOrderByNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(orderNumber))
                {
                    return ApiResponse<OrderResponseDto?>.Fail(
                        "Order number cannot be empty.", 
                        null, 
                        400
                    );
                }

                var order = await _orderRepository.GetByOrderNumberAsync(orderNumber, cancellationToken);
                
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

        public async Task<ApiResponse<IEnumerable<OrderResponseDto>>> GetOrdersByStatusAsync(string status, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(status))
                {
                    return ApiResponse<IEnumerable<OrderResponseDto>>.Fail(
                        "Status cannot be empty.", 
                        null, 
                        400
                    );
                }

                var orders = await _orderRepository.GetOrdersByStatusAsync(status, cancellationToken);
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
                    "An error occurred while retrieving orders by status.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<OrderResponseDto>> CreateOrderAsync(string userId, CreateOrderDto createOrderDto, CancellationToken cancellationToken = default)
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
                var address = await _customerAddressRepository.GetByIdAsync(createOrderDto.DeliveryAddressId, cancellationToken);
                if (address == null || address.UserId != userId || !address.IsActive)
                {
                    return ApiResponse<OrderResponseDto>.Fail(
                        "Invalid delivery address.", 
                        null, 
                        400
                    );
                }

                // Validate order items and calculate totals
                decimal subtotal = 0;
                var orderItems = new List<OrderItems>();

                foreach (var itemDto in createOrderDto.OrderItems)
                {
                    var product = await _productsRepository.GetByIdAsync(itemDto.ProductId, cancellationToken);
                    if (product == null || !product.IsActive)
                    {
                        return ApiResponse<OrderResponseDto>.Fail(
                            $"Product with ID {itemDto.ProductId} not found or not available.", 
                            null, 
                            400
                        );
                    }

                    if (product.StockQuantity < itemDto.Quantity)
                    {
                        return ApiResponse<OrderResponseDto>.Fail(
                            $"Insufficient stock for product {product.ProductName}. Available: {product.StockQuantity}, Requested: {itemDto.Quantity}", 
                            null, 
                            400
                        );
                    }

                    var orderItem = new OrderItems
                    {
                        ProductId = itemDto.ProductId,
                        Quantity = itemDto.Quantity,
                        UnitPrice = product.Price,
                        DiscountPercent = 0,
                        DiscountAmount = 0,
                        TotalPrice = product.Price * itemDto.Quantity,
                        Notes = itemDto.Notes
                    };

                    orderItems.Add(orderItem);
                    subtotal += orderItem.TotalPrice;
                }

                // Apply coupons if provided
                decimal couponDiscountAmount = 0;
                var appliedCoupons = new List<string>();

                if (createOrderDto.CouponCodes != null && createOrderDto.CouponCodes.Any())
                {
                    foreach (var couponCode in createOrderDto.CouponCodes)
                    {
                        var coupon = await _couponRepository.GetByCouponCodeAsync(couponCode, cancellationToken);
                        if (coupon != null && await _couponRepository.IsCouponValidAsync(couponCode, subtotal, cancellationToken))
                        {
                            decimal discount = 0;
                            if (coupon.DiscountType == "Percentage")
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
                    DeliveryAddressId = createOrderDto.DeliveryAddressId,
                    OrderDate = DateTime.UtcNow,
                    Subtotal = subtotal,
                    CouponDiscountAmount = couponDiscountAmount,
                    DeliveryCharges = deliveryCharges,
                    TotalAmount = subtotal - couponDiscountAmount + deliveryCharges,
                    OrderStatus = "Pending",
                    PaymentType = createOrderDto.PaymentType,
                    PaymentStatus = "Pending",
                    AppliedCoupons = appliedCoupons.Any() ? string.Join(",", appliedCoupons) : null,
                    DeliveryNotes = createOrderDto.DeliveryNotes,
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
                    var product = await _productsRepository.GetByIdAsync(orderItem.ProductId, cancellationToken);
                    if (product != null)
                    {
                        var previousStock = product.StockQuantity;
                        var newStock = previousStock - orderItem.Quantity;
                        
                        await _productsRepository.UpdateStockAsync(orderItem.ProductId, newStock, cancellationToken);
                        
                        await _stockMovementRepository.AddStockMovementAsync(
                            orderItem.ProductId,
                            -orderItem.Quantity,
                            previousStock,
                            newStock,
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

                // Clear user's cart
                await _shoppingCartRepository.ClearUserCartAsync(userId, cancellationToken);
                await _shoppingCartRepository.SaveChangesAsync(cancellationToken);

                // Get the complete order for response
                var createdOrder = await _orderRepository.GetByIdWithDetailsAsync(order.Id, cancellationToken);
                var orderDto = _mapper.Map<OrderResponseDto>(createdOrder);
                
                return ApiResponse<OrderResponseDto>.Ok(
                    orderDto, 
                    "Order created successfully.", 
                    201
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<OrderResponseDto>.Fail(
                    "An error occurred while creating the order.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<OrderResponseDto?>> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto, CancellationToken cancellationToken = default)
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

                // Update only provided fields
                if (!string.IsNullOrWhiteSpace(updateOrderDto.OrderStatus))
                    order.OrderStatus = updateOrderDto.OrderStatus;
                
                if (!string.IsNullOrWhiteSpace(updateOrderDto.PaymentStatus))
                    order.PaymentStatus = updateOrderDto.PaymentStatus;
                
                if (!string.IsNullOrWhiteSpace(updateOrderDto.DeliveryNotes))
                    order.DeliveryNotes = updateOrderDto.DeliveryNotes;
                
                if (updateOrderDto.IsActive.HasValue)
                    order.IsActive = updateOrderDto.IsActive.Value;

                order.UpdatedDate = DateTime.UtcNow;

                _orderRepository.Update(order);
                await _orderRepository.SaveChangesAsync(cancellationToken);

                var orderDto = _mapper.Map<OrderResponseDto>(order);
                
                return ApiResponse<OrderResponseDto?>.Ok(
                    orderDto, 
                    "Order updated successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<OrderResponseDto?>.Fail(
                    "An error occurred while updating the order.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<bool>> CancelOrderAsync(int id, string userId, CancellationToken cancellationToken = default)
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

                if (order.OrderStatus != "Pending" && order.OrderStatus != "Confirmed")
                {
                    return ApiResponse<bool>.Fail(
                        "Order cannot be cancelled in its current status.", 
                        false, 
                        400
                    );
                }

                order.OrderStatus = "Cancelled";
                order.UpdatedDate = DateTime.UtcNow;

                // Restore stock for cancelled order
                foreach (var orderItem in order.OrderItems)
                {
                    var product = await _productsRepository.GetByIdAsync(orderItem.ProductId, cancellationToken);
                    if (product != null)
                    {
                        var previousStock = product.StockQuantity;
                        var newStock = previousStock + orderItem.Quantity;
                        
                        await _productsRepository.UpdateStockAsync(orderItem.ProductId, newStock, cancellationToken);
                        
                        await _stockMovementRepository.AddStockMovementAsync(
                            orderItem.ProductId,
                            orderItem.Quantity,
                            previousStock,
                            newStock,
                            "CANCELLATION",
                            order.Id,
                            orderItem.UnitPrice,
                            $"Order {order.OrderNumber} cancelled",
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

        public async Task<ApiResponse<decimal>> GetTotalSalesAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var totalSales = await _orderRepository.GetTotalSalesAsync(startDate, endDate, cancellationToken);
                
                return ApiResponse<decimal>.Ok(
                    totalSales, 
                    "Total sales retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<decimal>.Fail(
                    "An error occurred while retrieving total sales.", 
                    0, 
                    500
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<OrderResponseDto>>> GetPendingOrdersAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var orders = await _orderRepository.GetPendingOrdersAsync(cancellationToken);
                var orderDtos = _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
                
                return ApiResponse<IEnumerable<OrderResponseDto>>.Ok(
                    orderDtos, 
                    "Pending orders retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<OrderResponseDto>>.Fail(
                    "An error occurred while retrieving pending orders.", 
                    null, 
                    500
                );
            }
        }

        private static decimal CalculateDeliveryCharges(decimal orderTotal, decimal? distanceKm)
        {
            // Free delivery for orders over $100
            if (orderTotal >= 100)
                return 0;

            // Base delivery charge
            decimal baseCharge = 10;

            // Additional charge based on distance
            if (distanceKm.HasValue && distanceKm > 10)
            {
                baseCharge += (distanceKm.Value - 10) * 2; // $2 per km over 10km
            }

            return Math.Min(baseCharge, 50); // Cap at $50
        }
    }
}