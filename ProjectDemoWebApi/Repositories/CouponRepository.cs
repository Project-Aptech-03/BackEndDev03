using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class CouponRepository : BaseRepository<Coupons>, ICouponRepository
    {
        public CouponRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Coupons?> GetByCouponCodeAsync(string couponCode, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(c => c.CouponCode == couponCode, cancellationToken);
        }

        public async Task<IEnumerable<Coupons>> GetActiveCouponsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.CouponName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Coupons>> GetValidCouponsAsync(DateTime currentDate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(c => c.IsActive && 
                           c.StartDate <= currentDate && 
                           c.EndDate >= currentDate &&
                           (c.Quantity > 0 || c.Quantity == -1))
                .OrderBy(c => c.CouponName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Coupons>> GetAutoApplyCouponsAsync(decimal orderAmount, CancellationToken cancellationToken = default)
        {
            var currentDate = DateTime.UtcNow;
            
            return await _dbSet.AsNoTracking()
                .Where(c => c.IsActive && 
                           c.IsAutoApply &&
                           c.StartDate <= currentDate && 
                           c.EndDate >= currentDate &&
                           c.MinOrderAmount <= orderAmount &&
                           (c.Quantity > 0 || c.Quantity == -1))
                .OrderByDescending(c => c.DiscountValue)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsCouponCodeExistsAsync(string couponCode, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable();
            
            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);
                
            return await query.AnyAsync(c => c.CouponCode == couponCode, cancellationToken);
        }

        public async Task<bool> IsCouponValidAsync(string couponCode, decimal orderAmount, CancellationToken cancellationToken = default)
        {
            var currentDate = DateTime.UtcNow;
            
            return await _dbSet.AnyAsync(c => 
                c.CouponCode == couponCode &&
                c.IsActive && 
                c.StartDate <= currentDate && 
                c.EndDate >= currentDate &&
                c.MinOrderAmount <= orderAmount &&
                (c.Quantity > 0 || c.Quantity == -1), cancellationToken);
        }

        public async Task DecrementCouponQuantityAsync(int couponId, CancellationToken cancellationToken = default)
        {
            var coupon = await _dbSet.FindAsync(new object[] { couponId }, cancellationToken);
            if (coupon != null && coupon.Quantity > 0)
            {
                coupon.Quantity--;
                _dbSet.Update(coupon);
            }
        }
    }
}