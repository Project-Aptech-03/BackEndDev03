using ProjectDemoWebApi.DTOs.Coupon;
using ProjectDemoWebApi.DTOs.Shared;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface ICouponService
    {
        Task<ApiResponse<IEnumerable<CouponResponseDto>>> GetAllCouponsAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<CouponResponseDto?>> GetCouponByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ApiResponse<CouponResponseDto?>> GetCouponByCodeAsync(string couponCode, CancellationToken cancellationToken = default);
        Task<ApiResponse<IEnumerable<CouponResponseDto>>> GetActiveCouponsAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<IEnumerable<CouponResponseDto>>> GetValidCouponsAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<IEnumerable<CouponResponseDto>>> GetAutoApplyCouponsAsync(decimal orderAmount, CancellationToken cancellationToken = default);
        Task<ApiResponse<CouponResponseDto>> CreateCouponAsync(CreateCouponDto createCouponDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<CouponResponseDto?>> UpdateCouponAsync(int id, UpdateCouponDto updateCouponDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeleteCouponAsync(int id, CancellationToken cancellationToken = default);
        Task<ApiResponse<CouponDiscountResultDto>> ValidateCouponAsync(ValidateCouponDto validateCouponDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<CouponDiscountResultDto>> ApplyCouponAsync(ApplyCouponDto applyCouponDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<decimal>> CalculateDiscountAsync(string couponCode, decimal orderAmount, CancellationToken cancellationToken = default);
    }
}