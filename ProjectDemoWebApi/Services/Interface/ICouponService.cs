using ProjectDemoWebApi.DTOs.Coupon;
using ProjectDemoWebApi.DTOs.Shared;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface ICouponService
    {
        Task<ApiResponse<IEnumerable<CouponResponseDto>>> GetAllCouponsAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<CouponResponseDto>> CreateCouponAsync(CreateCouponDto createCouponDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<CouponResponseDto?>> UpdateCouponAsync(int id, UpdateCouponDto updateCouponDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeleteCouponAsync(int id, CancellationToken cancellationToken = default);
        Task<ApiResponse<CouponDiscountResultDto>> ApplyCouponAsync(ApplyCouponDto applyCouponDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> UseCouponAsync(string couponCode, CancellationToken cancellationToken = default);
    }
}