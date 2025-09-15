using AutoMapper;
using ProjectDemoWebApi.DTOs.Coupon;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Services
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper _mapper;

        public CouponService(ICouponRepository couponRepository, IMapper mapper)
        {
            _couponRepository = couponRepository ?? throw new ArgumentNullException(nameof(couponRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ApiResponse<IEnumerable<CouponResponseDto>>> GetAllCouponsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var coupons = await _couponRepository.GetAllAsync(cancellationToken);
                var couponDtos = _mapper.Map<IEnumerable<CouponResponseDto>>(coupons);

                return ApiResponse<IEnumerable<CouponResponseDto>>.Ok(
                    couponDtos,
                    "Coupon list retrieved successfully.",
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CouponResponseDto>>.Fail(
                    "An error occurred while retrieving the coupon list.",
                    null,
                    500
                );
            }
        }

        public async Task<ApiResponse<CouponResponseDto>> CreateCouponAsync(CreateCouponDto createCouponDto, CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if coupon code already exists
                var existingCoupon = await _couponRepository.IsCouponCodeExistsAsync(createCouponDto.CouponCode, null, cancellationToken);
                if (existingCoupon)
                {
                    return ApiResponse<CouponResponseDto>.Fail(
                        "Coupon code already exists.",
                        null,
                        400
                    );
                }

                // Validate date range
                if (createCouponDto.EndDate <= createCouponDto.StartDate)
                {
                    return ApiResponse<CouponResponseDto>.Fail(
                        "End date must be after start date.",
                        null,
                        400
                    );
                }

                // Validate percentage discount
                if (createCouponDto.DiscountType == "percentage" && createCouponDto.DiscountValue > 100)
                {
                    return ApiResponse<CouponResponseDto>.Fail(
                        "Percentage discount value cannot exceed 100%.",
                        null,
                        400
                    );
                }

                var coupon = _mapper.Map<Coupons>(createCouponDto);
                coupon.CreatedDate = DateTime.UtcNow;

                await _couponRepository.AddAsync(coupon, cancellationToken);
                await _couponRepository.SaveChangesAsync(cancellationToken);

                var couponDto = _mapper.Map<CouponResponseDto>(coupon);

                return ApiResponse<CouponResponseDto>.Ok(
                    couponDto,
                    "Coupon created successfully.",
                    201
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<CouponResponseDto>.Fail(
                    "An error occurred while creating the coupon.",
                    null,
                    500
                );
            }
        }

        public async Task<ApiResponse<CouponResponseDto?>> UpdateCouponAsync(int id, UpdateCouponDto updateCouponDto, CancellationToken cancellationToken = default)
        {
            try
            {
                var coupon = await _couponRepository.GetByIdAsync(id, cancellationToken);

                if (coupon == null)
                {
                    return ApiResponse<CouponResponseDto?>.Fail(
                        "Coupon not found.",
                        null,
                        404
                    );
                }

                // Validate date range
                if (updateCouponDto.EndDate <= updateCouponDto.StartDate)
                {
                    return ApiResponse<CouponResponseDto?>.Fail(
                        "End date must be after start date.",
                        null,
                        400
                    );
                }

                // Validate percentage discount
                if (updateCouponDto.DiscountType == "percentage" && updateCouponDto.DiscountValue > 100)
                {
                    return ApiResponse<CouponResponseDto?>.Fail(
                        "Percentage discount value cannot exceed 100%.",
                        null,
                        400
                    );
                }

                _mapper.Map(updateCouponDto, coupon);

                _couponRepository.Update(coupon);
                await _couponRepository.SaveChangesAsync(cancellationToken);

                var couponDto = _mapper.Map<CouponResponseDto>(coupon);

                return ApiResponse<CouponResponseDto?>.Ok(
                    couponDto,
                    "Coupon updated successfully.",
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<CouponResponseDto?>.Fail(
                    "An error occurred while updating the coupon.",
                    null,
                    500
                );
            }
        }

        public async Task<ApiResponse<bool>> DeleteCouponAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var coupon = await _couponRepository.GetByIdAsync(id, cancellationToken);

                if (coupon == null)
                {
                    return ApiResponse<bool>.Fail(
                        "Coupon not found.",
                        false,
                        404
                    );
                }

                _couponRepository.Delete(coupon);
                await _couponRepository.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Ok(
                    true,
                    "Coupon deleted successfully.",
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(
                    "An error occurred while deleting the coupon.",
                    false,
                    500
                );
            }
        }

        public async Task<ApiResponse<CouponDiscountResultDto>> ApplyCouponAsync(ApplyCouponDto applyCouponDto, CancellationToken cancellationToken = default)
        {
            try
            {
                var coupon = await _couponRepository.GetByCouponCodeAsync(applyCouponDto.CouponCode, cancellationToken);

                if (coupon == null)
                {
                    return ApiResponse<CouponDiscountResultDto>.Fail(
                        "Coupon code does not exist.",
                        null,
                        404
                    );
                }

                var validationResult = ValidateCouponInternal(coupon, applyCouponDto.OrderAmount);

                if (!validationResult.IsValid)
                {
                    return ApiResponse<CouponDiscountResultDto>.Fail(
                        validationResult.Message,
                        null,
                        400
                    );
                }

                var couponDto = _mapper.Map<CouponResponseDto>(coupon);
                validationResult.CouponInfo = couponDto;

                return ApiResponse<CouponDiscountResultDto>.Ok(
                    validationResult,
                    "Valid coupon applied successfully.",
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<CouponDiscountResultDto>.Fail(
                    "An error occurred while applying the coupon.",
                    null,
                    500
                );
            }
        }

        public async Task<ApiResponse<bool>> UseCouponAsync(string couponCode, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(couponCode))
                {
                    return ApiResponse<bool>.Fail(
                        "Coupon code cannot be empty.",
                        false,
                        400
                    );
                }

                var coupon = await _couponRepository.GetByCouponCodeAsync(couponCode, cancellationToken);

                if (coupon == null)
                {
                    return ApiResponse<bool>.Fail(
                        "Coupon code does not exist.",
                        false,
                        404
                    );
                }

                // Check if the coupon has quantity left
                if (coupon.Quantity == 0)
                {
                    return ApiResponse<bool>.Fail(
                        "Coupon is out of stock.",
                        false,
                        400
                    );
                }

                // Decrease the coupon quantity
                if (coupon.Quantity > 0) // Do not decrease if it is unlimited (-1)
                {
                    coupon.Quantity -= 1;
                    _couponRepository.Update(coupon);
                    await _couponRepository.SaveChangesAsync(cancellationToken);
                }

                return ApiResponse<bool>.Ok(
                    true,
                    "Coupon used successfully.",
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(
                    "An error occurred while using the coupon.",
                    false,
                    500
                );
            }
        }

        private CouponDiscountResultDto ValidateCouponInternal(Coupons coupon, decimal orderAmount)
        {
            var currentDate = DateTime.UtcNow;

            // Check if coupon is active
            if (!coupon.IsActive)
            {
                return new CouponDiscountResultDto
                {
                    IsValid = false,
                    Message = "Coupon is no longer active.",
                    DiscountAmount = 0,
                    FinalAmount = orderAmount
                };
            }

            // Check date validity
            if (currentDate < coupon.StartDate)
            {
                return new CouponDiscountResultDto
                {
                    IsValid = false,
                    Message = "Coupon is not yet valid.",
                    DiscountAmount = 0,
                    FinalAmount = orderAmount
                };
            }

            if (currentDate > coupon.EndDate)
            {
                return new CouponDiscountResultDto
                {
                    IsValid = false,
                    Message = "Coupon has expired.",
                    DiscountAmount = 0,
                    FinalAmount = orderAmount
                };
            }

            // Check quantity
            if (coupon.Quantity == 0)
            {
                return new CouponDiscountResultDto
                {
                    IsValid = false,
                    Message = "Coupon is out of stock.",
                    DiscountAmount = 0,
                    FinalAmount = orderAmount
                };
            }

            // Check minimum order amount
            if (orderAmount < coupon.MinOrderAmount)
            {
                return new CouponDiscountResultDto
                {
                    IsValid = false,
                    Message = $"Order must have a minimum value of {coupon.MinOrderAmount:C} to use this coupon.",
                    DiscountAmount = 0,
                    FinalAmount = orderAmount
                };
            }

            // Calculate discount
            decimal discountAmount;
            if (coupon.DiscountType == "percentage")
            {
                discountAmount = orderAmount * (coupon.DiscountValue / 100);
            }
            else // fixed
            {
                discountAmount = coupon.DiscountValue;
            }

            // Apply max discount limit
            if (coupon.MaxDiscountAmount > 0 && discountAmount > coupon.MaxDiscountAmount)
            {
                discountAmount = coupon.MaxDiscountAmount;
            }

            // Ensure discount doesn't exceed order amount
            if (discountAmount > orderAmount)
            {
                discountAmount = orderAmount;
            }

            return new CouponDiscountResultDto
            {
                IsValid = true,
                Message = "Valid coupon applied successfully.",
                DiscountAmount = discountAmount,
                FinalAmount = orderAmount - discountAmount
            };
        }
    }
}
