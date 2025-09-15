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
                    "Danh s�ch coupon ?� ???c l?y th�nh c�ng.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CouponResponseDto>>.Fail(
                    "?� x?y ra l?i khi l?y danh s�ch coupon.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<CouponResponseDto?>> GetCouponByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var coupon = await _couponRepository.GetByIdAsync(id, cancellationToken);
                
                if (coupon == null)
                {
                    return ApiResponse<CouponResponseDto?>.Fail(
                        "Kh�ng t�m th?y coupon.", 
                        null, 
                        404
                    );
                }

                var couponDto = _mapper.Map<CouponResponseDto>(coupon);
                
                return ApiResponse<CouponResponseDto?>.Ok(
                    couponDto, 
                    "Coupon ?� ???c l?y th�nh c�ng.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<CouponResponseDto?>.Fail(
                    "?� x?y ra l?i khi l?y th�ng tin coupon.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<CouponResponseDto?>> GetCouponByCodeAsync(string couponCode, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(couponCode))
                {
                    return ApiResponse<CouponResponseDto?>.Fail(
                        "M� coupon kh�ng ???c ?? tr?ng.", 
                        null, 
                        400
                    );
                }

                var coupon = await _couponRepository.GetByCouponCodeAsync(couponCode, cancellationToken);
                
                if (coupon == null)
                {
                    return ApiResponse<CouponResponseDto?>.Fail(
                        "Kh�ng t�m th?y coupon v?i m� n�y.", 
                        null, 
                        404
                    );
                }

                var couponDto = _mapper.Map<CouponResponseDto>(coupon);
                
                return ApiResponse<CouponResponseDto?>.Ok(
                    couponDto, 
                    "Coupon ?� ???c l?y th�nh c�ng.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<CouponResponseDto?>.Fail(
                    "?� x?y ra l?i khi l?y th�ng tin coupon.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<CouponResponseDto>>> GetActiveCouponsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var coupons = await _couponRepository.GetActiveCouponsAsync(cancellationToken);
                var couponDtos = _mapper.Map<IEnumerable<CouponResponseDto>>(coupons);

                return ApiResponse<IEnumerable<CouponResponseDto>>.Ok(
                    couponDtos, 
                    "Danh s�ch coupon ?ang ho?t ??ng ?� ???c l?y th�nh c�ng.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CouponResponseDto>>.Fail(
                    "?� x?y ra l?i khi l?y danh s�ch coupon ?ang ho?t ??ng.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<CouponResponseDto>>> GetValidCouponsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var coupons = await _couponRepository.GetValidCouponsAsync(DateTime.UtcNow, cancellationToken);
                var couponDtos = _mapper.Map<IEnumerable<CouponResponseDto>>(coupons);

                return ApiResponse<IEnumerable<CouponResponseDto>>.Ok(
                    couponDtos, 
                    "Danh s�ch coupon h?p l? ?� ???c l?y th�nh c�ng.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CouponResponseDto>>.Fail(
                    "?� x?y ra l?i khi l?y danh s�ch coupon h?p l?.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<CouponResponseDto>>> GetAutoApplyCouponsAsync(decimal orderAmount, CancellationToken cancellationToken = default)
        {
            try
            {
                var coupons = await _couponRepository.GetAutoApplyCouponsAsync(orderAmount, cancellationToken);
                var couponDtos = _mapper.Map<IEnumerable<CouponResponseDto>>(coupons);

                return ApiResponse<IEnumerable<CouponResponseDto>>.Ok(
                    couponDtos, 
                    "Danh s�ch coupon t? ??ng �p d?ng ?� ???c l?y th�nh c�ng.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CouponResponseDto>>.Fail(
                    "?� x?y ra l?i khi l?y danh s�ch coupon t? ??ng �p d?ng.", 
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
                        "M� coupon ?� t?n t?i.", 
                        null, 
                        400
                    );
                }

                // Validate date range
                if (createCouponDto.EndDate <= createCouponDto.StartDate)
                {
                    return ApiResponse<CouponResponseDto>.Fail(
                        "Ng�y k?t th�c ph?i sau ng�y b?t ??u.", 
                        null, 
                        400
                    );
                }

                // Validate percentage discount
                if (createCouponDto.DiscountType == "percentage" && createCouponDto.DiscountValue > 100)
                {
                    return ApiResponse<CouponResponseDto>.Fail(
                        "Gi� tr? gi?m gi� theo ph?n tr?m kh�ng ???c v??t qu� 100%.", 
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
                    "Coupon ?� ???c t?o th�nh c�ng.", 
                    201
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<CouponResponseDto>.Fail(
                    "?� x?y ra l?i khi t?o coupon.", 
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
                        "Kh�ng t�m th?y coupon.", 
                        null, 
                        404
                    );
                }

                // Validate date range
                if (updateCouponDto.EndDate <= updateCouponDto.StartDate)
                {
                    return ApiResponse<CouponResponseDto?>.Fail(
                        "Ng�y k?t th�c ph?i sau ng�y b?t ??u.", 
                        null, 
                        400
                    );
                }

                // Validate percentage discount
                if (updateCouponDto.DiscountType == "percentage" && updateCouponDto.DiscountValue > 100)
                {
                    return ApiResponse<CouponResponseDto?>.Fail(
                        "Gi� tr? gi?m gi� theo ph?n tr?m kh�ng ???c v??t qu� 100%.", 
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
                    "Coupon ?� ???c c?p nh?t th�nh c�ng.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<CouponResponseDto?>.Fail(
                    "?� x?y ra l?i khi c?p nh?t coupon.", 
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
                        "Kh�ng t�m th?y coupon.", 
                        false, 
                        404
                    );
                }

                _couponRepository.Delete(coupon);
                await _couponRepository.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Ok(
                    true, 
                    "Coupon ?� ???c x�a th�nh c�ng.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(
                    "?� x?y ra l?i khi x�a coupon.", 
                    false, 
                    500
                );
            }
        }

        public async Task<ApiResponse<CouponDiscountResultDto>> ValidateCouponAsync(ValidateCouponDto validateCouponDto, CancellationToken cancellationToken = default)
        {
            try
            {
                var coupon = await _couponRepository.GetByCouponCodeAsync(validateCouponDto.CouponCode, cancellationToken);
                
                if (coupon == null)
                {
                    return ApiResponse<CouponDiscountResultDto>.Ok(
                        new CouponDiscountResultDto
                        {
                            IsValid = false,
                            Message = "M� coupon kh�ng t?n t?i.",
                            DiscountAmount = 0,
                            FinalAmount = validateCouponDto.OrderAmount
                        },
                        "M� coupon kh�ng h?p l?.",
                        200
                    );
                }

                var validationResult = ValidateCouponInternal(coupon, validateCouponDto.OrderAmount);
                var couponDto = _mapper.Map<CouponResponseDto>(coupon);
                validationResult.CouponInfo = couponDto;

                return ApiResponse<CouponDiscountResultDto>.Ok(
                    validationResult,
                    validationResult.IsValid ? "Coupon h?p l?." : validationResult.Message,
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<CouponDiscountResultDto>.Fail(
                    "?� x?y ra l?i khi x�c th?c coupon.", 
                    null, 
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
                        "M� coupon kh�ng t?n t?i.", 
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
                    "Coupon ?� ???c �p d?ng th�nh c�ng.",
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<CouponDiscountResultDto>.Fail(
                    "?� x?y ra l?i khi �p d?ng coupon.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<decimal>> CalculateDiscountAsync(string couponCode, decimal orderAmount, CancellationToken cancellationToken = default)
        {
            try
            {
                var coupon = await _couponRepository.GetByCouponCodeAsync(couponCode, cancellationToken);
                
                if (coupon == null)
                {
                    return ApiResponse<decimal>.Fail(
                        "M� coupon kh�ng t?n t?i.", 
                        0, 
                        404
                    );
                }

                var validationResult = ValidateCouponInternal(coupon, orderAmount);
                
                if (!validationResult.IsValid)
                {
                    return ApiResponse<decimal>.Fail(
                        validationResult.Message, 
                        0, 
                        400
                    );
                }

                return ApiResponse<decimal>.Ok(
                    validationResult.DiscountAmount,
                    "S? ti?n gi?m gi� ?� ???c t�nh to�n.",
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<decimal>.Fail(
                    "?� x?y ra l?i khi t�nh to�n gi?m gi�.", 
                    0, 
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
                    Message = "Coupon kh�ng c�n ho?t ??ng.",
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
                    Message = "Coupon ch?a c� hi?u l?c.",
                    DiscountAmount = 0,
                    FinalAmount = orderAmount
                };
            }

            if (currentDate > coupon.EndDate)
            {
                return new CouponDiscountResultDto
                {
                    IsValid = false,
                    Message = "Coupon ?� h?t h?n.",
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
                    Message = "Coupon ?� h?t s? l??ng.",
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
                    Message = $"??n h�ng ph?i c� gi� tr? t?i thi?u {coupon.MinOrderAmount:C} ?? s? d?ng coupon n�y.",
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
                Message = "Coupon h?p l? v� ?� ???c �p d?ng.",
                DiscountAmount = discountAmount,
                FinalAmount = orderAmount - discountAmount
            };
        }
    }
}