using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectDemoWebApi.DTOs.Coupon;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponsController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        /// <summary>
        /// L?y danh sách t?t c? coupon (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCoupons()
        {
            var result = await _couponService.GetAllCouponsAsync();
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// L?y thông tin coupon theo ID (Admin only)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCouponById(int id)
        {
            var result = await _couponService.GetCouponByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// L?y thông tin coupon theo mã code
        /// </summary>
        [HttpGet("code/{couponCode}")]
        [Authorize]
        public async Task<IActionResult> GetCouponByCode(string couponCode)
        {
            var result = await _couponService.GetCouponByCodeAsync(couponCode);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// L?y danh sách coupon ?ang ho?t ??ng
        /// </summary>
        [HttpGet("active")]
        [Authorize]
        public async Task<IActionResult> GetActiveCoupons()
        {
            var result = await _couponService.GetActiveCouponsAsync();
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// L?y danh sách coupon h?p l? (ch?a h?t h?n, còn s? l??ng)
        /// </summary>
        [HttpGet("valid")]
        [Authorize]
        public async Task<IActionResult> GetValidCoupons()
        {
            var result = await _couponService.GetValidCouponsAsync();
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// L?y danh sách coupon t? ??ng áp d?ng theo s? ti?n ??n hàng
        /// </summary>
        [HttpGet("auto-apply/{orderAmount}")]
        [Authorize]
        public async Task<IActionResult> GetAutoApplyCoupons(decimal orderAmount)
        {
            var result = await _couponService.GetAutoApplyCouponsAsync(orderAmount);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// T?o coupon m?i (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCoupon(CreateCouponDto createCouponDto)
        {
            var result = await _couponService.CreateCouponAsync(createCouponDto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// C?p nh?t thông tin coupon (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCoupon(int id, UpdateCouponDto updateCouponDto)
        {
            var result = await _couponService.UpdateCouponAsync(id, updateCouponDto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Xóa coupon (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var result = await _couponService.DeleteCouponAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Xác th?c mã coupon v?i s? ti?n ??n hàng
        /// </summary>
        [HttpPost("validate")]
        [Authorize]
        public async Task<IActionResult> ValidateCoupon(ValidateCouponDto validateCouponDto)
        {
            var result = await _couponService.ValidateCouponAsync(validateCouponDto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Áp d?ng coupon và tính toán gi?m giá
        /// </summary>
        [HttpPost("apply")]
        [Authorize]
        public async Task<IActionResult> ApplyCoupon(ApplyCouponDto applyCouponDto)
        {
            var result = await _couponService.ApplyCouponAsync(applyCouponDto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Tính toán s? ti?n gi?m giá c?a coupon
        /// </summary>
        [HttpPost("calculate-discount")]
        [Authorize]
        public async Task<IActionResult> CalculateDiscount([FromBody] ApplyCouponDto applyCouponDto)
        {
            var result = await _couponService.CalculateDiscountAsync(applyCouponDto.CouponCode, applyCouponDto.OrderAmount);
            return StatusCode(result.StatusCode, result);
        }
    }
}