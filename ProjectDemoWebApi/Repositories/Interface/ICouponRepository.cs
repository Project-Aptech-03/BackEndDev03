using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface ICouponRepository : IBaseRepository<Coupons>
    {
        Task<Coupons?> GetByCouponCodeAsync(string couponCode, CancellationToken cancellationToken = default);
        Task<IEnumerable<Coupons>> GetActiveCouponsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Coupons>> GetValidCouponsAsync(DateTime currentDate, CancellationToken cancellationToken = default);
        Task<IEnumerable<Coupons>> GetAutoApplyCouponsAsync(decimal orderAmount, CancellationToken cancellationToken = default);
        Task<bool> IsCouponCodeExistsAsync(string couponCode, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<bool> IsCouponValidAsync(string couponCode, decimal orderAmount, CancellationToken cancellationToken = default);
        Task DecrementCouponQuantityAsync(int couponId, CancellationToken cancellationToken = default);
    }
}