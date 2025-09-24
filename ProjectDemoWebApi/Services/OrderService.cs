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
        private readonly ICouponRepository _couponRepository;
        private readonly ISePayService _sePayService;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IProductsRepository productsRepository,
            ICustomerAddressRepository customerAddressRepository,
            IShoppingCartRepository shoppingCartRepository,
            ICouponRepository couponRepository,
            ISePayService sePayService,
            IMapper mapper)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _orderItemRepository = orderItemRepository ?? throw new ArgumentNullException(nameof(orderItemRepository));
            _productsRepository = productsRepository ?? throw new ArgumentNullException(nameof(productsRepository));
            _customerAddressRepository = customerAddressRepository ?? throw new ArgumentNullException(nameof(customerAddressRepository));
            _shoppingCartRepository = shoppingCartRepository ?? throw new ArgumentNullException(nameof(shoppingCartRepository));
            _couponRepository = couponRepository ?? throw new ArgumentNullException(nameof(couponRepository));
            _sePayService = sePayService ?? throw new ArgumentNullException(nameof(sePayService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #region Admin Functions

        public async Task<ApiResponse<IEnumerable<OrderResponseDto>>> GetAllOrdersAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var orders = await _orderRepository.GetAllOrdersWithDetailsAsync(cancellationToken);
                var orderDtos = _mapper.Map<IEnumerable<OrderResponseDto>>(orders);

                return ApiResponse<IEnumerable<OrderResponseDto>>.Ok(orderDtos, "All orders retrieved successfully.");
            }
            catch (Exception)
            {
                return ApiResponse<IEnumerable<OrderResponseDto>>.Fail("An error occurred while retrieving all orders.", null, 500);
            }
        }

        public async Task<ApiResponse<OrderResponseDto?>> UpdateOrderStatusAsync(int id, UpdateOrderDto updateOrderDto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return ApiResponse<OrderResponseDto?>.Fail("Invalid order ID.", null, 400);

                var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
                if (order == null)
                    return ApiResponse<OrderResponseDto?>.Fail("Order not found.", null, 404);

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

                return ApiResponse<OrderResponseDto?>.Ok(orderDto, "Order status updated successfully.");
            }
            catch (Exception)
            {
                return ApiResponse<OrderResponseDto?>.Fail("An error occurred while updating the order status.", null, 500);
            }
        }

        #endregion

        #region Customer Functions

        public async Task<ApiResponse<OrderResponseDto>> CreateOrderAsync(string userId, CreateOrderDto createOrderDto, CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate input
                var validationResult = ValidateCreateOrderInput(userId, createOrderDto);
                if (!validationResult.Success)
                    return validationResult;

                // Validate delivery address
                var addressValidation = await ValidateDeliveryAddressAsync(userId, createOrderDto.DeliveryAddressId, cancellationToken);
                if (!addressValidation.Success)
                    return addressValidation;

                // Process order items and calculate subtotal
                var orderItemsResult = await ProcessOrderItemsAsync(createOrderDto.OrderItems, cancellationToken);
                if (!orderItemsResult.Success)
                    return ApiResponse<OrderResponseDto>.Fail(orderItemsResult.Message, null, orderItemsResult.StatusCode);

                var (orderItems, orderedProductIds, subtotal) = orderItemsResult.Data;

                // Apply coupons
                var (couponDiscountAmount, appliedCoupons) = await ProcessCouponsAsync(createOrderDto.CouponCodes, subtotal, cancellationToken);

                // Create order
                var order = await CreateOrderEntityAsync(userId, createOrderDto, subtotal, couponDiscountAmount, appliedCoupons, cancellationToken);

                // Check payment if PaymentType is "BankTransfer"
                if (string.Equals(order.PaymentType, "BankTransfer", StringComparison.OrdinalIgnoreCase))
                {
                    var paymentResult = await _sePayService.FindTransactionAsync(order.OrderNumber, order.TotalAmount);

                    if (paymentResult.Success && paymentResult.Data != null)
                    {
                        // Payment found - update payment status
                        order.PaymentStatus = "Confirmed";
                    }
                    else
                    {
                        // Payment not found - cancel order
                        order.PaymentStatus = "Failed";
                        order.OrderStatus = "Cancelled";
                        order.CancellationReason = "Payment verification failed - No matching transaction found";
                        order.CancelledDate = DateTime.UtcNow;

                        return ApiResponse<OrderResponseDto>.Fail(
                            "Payment verification failed. Order has been cancelled.",
                            new List<string> { $"No transaction found for order {order.OrderNumber} with amount {order.TotalAmount:C}" },
                            400
                        );
                    }
                }

                // Save order and items
                await SaveOrderAndItemsAsync(order, orderItems, cancellationToken);

                // Update stock (reduce quantities)
                await UpdateProductStockAsync(orderItems, cancellationToken);

                // Remove items from shopping cart
                await RemoveItemsFromCartAsync(userId, orderedProductIds, cancellationToken);

                // Return response
                var createdOrder = await _orderRepository.GetByIdWithDetailsAsync(order.Id, cancellationToken);
                var orderDto = _mapper.Map<OrderResponseDto>(createdOrder);

                return ApiResponse<OrderResponseDto>.Ok(orderDto, "Order created successfully.", 201);
            }
            catch (Exception)
            {
                return ApiResponse<OrderResponseDto>.Fail("An error occurred while creating the order.", null, 500);
            }
        }

        public async Task<ApiResponse<bool>> CancelOrderAsync(int id, string userId, CancelOrderDto cancelOrderDto, CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate input
                if (id <= 0)
                    return ApiResponse<bool>.Fail("Invalid order ID.", false, 400);

                if (cancelOrderDto == null)
                    return ApiResponse<bool>.Fail("Cancellation reason is required.", false, 400);

                // Get order with details (including OrderItems) - Use AsNoTracking for read
                var orderWithDetails = await _orderRepository.GetByIdWithDetailsAsync(id, cancellationToken);
                if (orderWithDetails == null)
                    return ApiResponse<bool>.Fail("Order not found.", false, 404);

                if (orderWithDetails.CustomerId != userId)
                    return ApiResponse<bool>.Fail("You can only cancel your own orders.", false, 403);

                // Check if cancellation is allowed
                if (!IsOrderCancellable(orderWithDetails.OrderStatus))
                    return ApiResponse<bool>.Fail($"Order cannot be cancelled. Current status: {orderWithDetails.OrderStatus}.", false, 400);

                // Get a tracked entity for update
                var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
                if (order == null)
                    return ApiResponse<bool>.Fail("Order not found.", false, 404);

                // Update order status
                order.OrderStatus = "Cancelled";
                order.CancellationReason = cancelOrderDto.CancellationReason;
                order.CancelledDate = DateTime.UtcNow;
                order.UpdatedDate = DateTime.UtcNow;

                // Restore stock for cancelled order (using the detailed order data)
                if (orderWithDetails.OrderItems != null && orderWithDetails.OrderItems.Any())
                {
                    await RestoreProductStockAsync(orderWithDetails.OrderItems, cancellationToken);
                }

                // Save changes
                _orderRepository.Update(order);
                await _orderRepository.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Ok(true, "Order cancelled successfully.");
            }
            catch (Exception)
            {
                return ApiResponse<bool>.Fail("An error occurred while cancelling the order.", false, 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<BestSellerProductDto>>> GetTop3ProductsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var orderItems = await _orderItemRepository.GetAllAsync(cancellationToken);

                var topProducts = orderItems
                    .GroupBy(oi => oi.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        TotalQuantity = g.Sum(x => x.Quantity)
                    })
                    .OrderByDescending(x => x.TotalQuantity)
                    .Take(4)
                    .ToList();

                var result = new List<BestSellerProductDto>();

                foreach (var item in topProducts)
                {
                    var product = await _productsRepository.GetByIdAsync(item.ProductId, cancellationToken);
                    if (product != null)
                    {
                        result.Add(new BestSellerProductDto
                        {
                            ProductId = product.Id,
                            ProductName = product.ProductName,
                            TotalQuantity = item.TotalQuantity
                        });
                    }
                }

                return ApiResponse<IEnumerable<BestSellerProductDto>>.Ok(result, "Top 3 products retrieved successfully.");
            }
            catch (Exception)
            {
                return ApiResponse<IEnumerable<BestSellerProductDto>>.Fail("An error occurred while retrieving top products.", null, 500);
            }
        }



        #endregion

        #region Common Functions

        public async Task<ApiResponse<OrderResponseDto?>> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return ApiResponse<OrderResponseDto?>.Fail("Invalid order ID.", null, 400);

                var order = await _orderRepository.GetByIdWithDetailsAsync(id, cancellationToken);
                if (order == null)
                    return ApiResponse<OrderResponseDto?>.Fail("Order not found.", null, 404);

                var orderDto = _mapper.Map<OrderResponseDto>(order);
                return ApiResponse<OrderResponseDto?>.Ok(orderDto, "Order retrieved successfully.");
            }
            catch (Exception)
            {
                return ApiResponse<OrderResponseDto?>.Fail("An error occurred while retrieving the order.", null, 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<OrderResponseDto>>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    return ApiResponse<IEnumerable<OrderResponseDto>>.Fail("User ID cannot be empty.", null, 400);

                var orders = await _orderRepository.GetUserOrdersAsync(userId, cancellationToken);
                var orderDtos = _mapper.Map<IEnumerable<OrderResponseDto>>(orders);

                return ApiResponse<IEnumerable<OrderResponseDto>>.Ok(orderDtos, "Orders retrieved successfully.");
            }
            catch (Exception)
            {
                return ApiResponse<IEnumerable<OrderResponseDto>>.Fail("An error occurred while retrieving orders.", null, 500);
            }
        }

        public async Task<ApiResponse<string>> GetNextOrderCodeAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var orderCode = await _orderRepository.GenerateOrderNumberAsync(cancellationToken);
                return ApiResponse<string>.Ok(orderCode, "Next order code generated successfully.");
            }
            catch (Exception)
            {
                return ApiResponse<string>.Fail("An error occurred while generating order code.", null, 500);
            }
        }

        #endregion

        #region Private Helper Methods

        private static ApiResponse<OrderResponseDto> ValidateCreateOrderInput(string userId, CreateOrderDto createOrderDto)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ApiResponse<OrderResponseDto>.Fail("User ID cannot be empty.", null, 400);

            if (createOrderDto?.OrderItems == null || !createOrderDto.OrderItems.Any())
                return ApiResponse<OrderResponseDto>.Fail("Order items cannot be empty.", null, 400);

            return ApiResponse<OrderResponseDto>.Ok(null!, "Valid");
        }

        private async Task<ApiResponse<OrderResponseDto>> ValidateDeliveryAddressAsync(string userId, int deliveryAddressId, CancellationToken cancellationToken)
        {
            var address = await _customerAddressRepository.GetByIdAsync(deliveryAddressId, cancellationToken);
            if (address == null || address.UserId != userId || !address.IsActive)
                return ApiResponse<OrderResponseDto>.Fail("Invalid delivery address.", null, 400);

            return ApiResponse<OrderResponseDto>.Ok(null!, "Valid");
        }

        private async Task<ApiResponse<(List<OrderItems> orderItems, List<int> productIds, decimal subtotal)>> ProcessOrderItemsAsync(
            List<CreateOrderItemDto> orderItemDtos, CancellationToken cancellationToken)
        {
            var orderItems = new List<OrderItems>();
            var orderedProductIds = new List<int>();
            decimal subtotal = 0;

            foreach (var orderItemDto in orderItemDtos)
            {
                var product = await _productsRepository.GetByIdNoTrackingAsync(orderItemDto.ProductId, cancellationToken);
                if (product == null || !product.IsActive)
                    return ApiResponse<(List<OrderItems>, List<int>, decimal)>.Fail(
                        $"Product with ID {orderItemDto.ProductId} not found or not available.", null, 400);

                if (product.StockQuantity < orderItemDto.Quantity)
                    return ApiResponse<(List<OrderItems>, List<int>, decimal)>.Fail(
                        $"Insufficient stock for product {product.ProductName}. Available: {product.StockQuantity}, Requested: {orderItemDto.Quantity}", null, 400);

                var orderItem = new OrderItems
                {
                    ProductId = orderItemDto.ProductId,
                    Quantity = orderItemDto.Quantity,
                    UnitPrice = product.Price,
                    TotalPrice = product.Price * orderItemDto.Quantity
                };

                orderItems.Add(orderItem);
                orderedProductIds.Add(orderItemDto.ProductId);
                subtotal += orderItem.TotalPrice;
            }

            return ApiResponse<(List<OrderItems>, List<int>, decimal)>.Ok((orderItems, orderedProductIds, subtotal), "Valid");
        }

        private async Task<(decimal couponDiscountAmount, List<string> appliedCoupons)> ProcessCouponsAsync(
            List<string>? couponCodes, decimal subtotal, CancellationToken cancellationToken)
        {
            decimal couponDiscountAmount = 0;
            var appliedCoupons = new List<string>();

            if (couponCodes?.Any() == true)
            {
                foreach (var couponCode in couponCodes)
                {
                    var coupon = await _couponRepository.GetByCouponCodeAsync(couponCode, cancellationToken);
                    if (coupon != null && await _couponRepository.IsCouponValidAsync(couponCode, subtotal, cancellationToken))
                    {
                        decimal discount = CalculateCouponDiscount(coupon, subtotal);
                        couponDiscountAmount += discount;
                        appliedCoupons.Add(couponCode);

                        await _couponRepository.DecrementCouponQuantityAsync(coupon.Id, cancellationToken);
                    }
                }
            }

            return (couponDiscountAmount, appliedCoupons);
        }

        private static decimal CalculateCouponDiscount(Coupons coupon, decimal subtotal)
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
            return discount;
        }

        private async Task<Orders> CreateOrderEntityAsync(string userId, CreateOrderDto createOrderDto, decimal subtotal,
            decimal couponDiscountAmount, List<string> appliedCoupons, CancellationToken cancellationToken)
        {
            return new Orders
            {
                OrderNumber = await _orderRepository.GenerateOrderNumberAsync(cancellationToken),
                CustomerId = userId,
                DeliveryAddressId = createOrderDto.DeliveryAddressId,
                OrderDate = DateTime.UtcNow,
                Subtotal = subtotal,
                CouponDiscountAmount = couponDiscountAmount,
                DeliveryCharges = createOrderDto.DeliveryCharges,
                TotalAmount = subtotal - couponDiscountAmount + createOrderDto.DeliveryCharges,
                OrderStatus = "Pending",
                PaymentType = createOrderDto.PaymentType,
                PaymentStatus = "Pending",
                AppliedCoupons = appliedCoupons.Any() ? string.Join(",", appliedCoupons) : null,
                DeliveryNotes = createOrderDto.DeliveryNotes,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };
        }

        private async Task SaveOrderAndItemsAsync(Orders order, List<OrderItems> orderItems, CancellationToken cancellationToken)
        {
            await _orderRepository.AddAsync(order, cancellationToken);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            foreach (var orderItem in orderItems)
                orderItem.OrderId = order.Id;

            await _orderItemRepository.AddRangeAsync(orderItems, cancellationToken);
            await _orderItemRepository.SaveChangesAsync(cancellationToken);
        }

        private async Task UpdateProductStockAsync(List<OrderItems> orderItems, CancellationToken cancellationToken)
        {
            foreach (var orderItem in orderItems)
            {
                var product = await _productsRepository.GetByIdAsync(orderItem.ProductId, cancellationToken);
                if (product != null)
                {
                    var newStock = product.StockQuantity - orderItem.Quantity;
                    await _productsRepository.UpdateStockAsync(orderItem.ProductId, newStock, cancellationToken);
                }
            }

            await _productsRepository.SaveChangesAsync(cancellationToken);
        }

        private async Task RestoreProductStockAsync(ICollection<OrderItems> orderItems, CancellationToken cancellationToken)
        {
            if (orderItems == null || !orderItems.Any())
                return;

            foreach (var orderItem in orderItems)
            {
                var product = await _productsRepository.GetByIdAsync(orderItem.ProductId, cancellationToken);
                if (product != null)
                {
                    var newStock = product.StockQuantity + orderItem.Quantity;
                    await _productsRepository.UpdateStockAsync(orderItem.ProductId, newStock, cancellationToken);
                }
            }

            await _productsRepository.SaveChangesAsync(cancellationToken);
        }

        private async Task RemoveItemsFromCartAsync(string userId, List<int> orderedProductIds, CancellationToken cancellationToken)
        {
            if (orderedProductIds.Any())
                await _shoppingCartRepository.RemoveProductsFromCartAsync(userId, orderedProductIds, cancellationToken);
        }

        private static bool IsOrderCancellable(string orderStatus)
        {
            var cancellableStatuses = new[] { "Pending", "Confirmed", "Processing" };
            return cancellableStatuses.Any(status => string.Equals(status, orderStatus, StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }
}