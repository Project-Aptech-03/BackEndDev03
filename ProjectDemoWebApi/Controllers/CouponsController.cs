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
        /// L?y danh s�ch t?t c? coupon (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCoupons()
        {
            var result = await _couponService.GetAllCouponsAsync();
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// L?y th�ng tin coupon theo ID (Admin only)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCouponById(int id)
        {
            var result = await _couponService.GetCouponByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// L?y th�ng tin coupon theo m� code
        /// </summary>
        [HttpGet("code/{couponCode}")]
        [Authorize]
        public async Task<IActionResult> GetCouponByCode(string couponCode)
        {
            var result = await _couponService.GetCouponByCodeAsync(couponCode);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// L?y danh s�ch coupon ?ang ho?t ??ng
        /// </summary>
        [HttpGet("active")]
        [Authorize]
        public async Task<IActionResult> GetActiveCoupons()
        {
            var result = await _couponService.GetActiveCouponsAsync();
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// L?y danh s�ch coupon h?p l? (ch?a h?t h?n, c�n s? l??ng)
        /// </summary>
        [HttpGet("valid")]
        [Authorize]
        public async Task<IActionResult> GetValidCoupons()
        {
            var result = await _couponService.GetValidCouponsAsync();
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// L?y danh s�ch coupon t? ??ng �p d?ng theo s? ti?n ??n h�ng
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
        /// C?p nh?t th�ng tin coupon (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCoupon(int id, UpdateCouponDto updateCouponDto)
        {
            var result = await _couponService.UpdateCouponAsync(id, updateCouponDto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// X�a coupon (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var result = await _couponService.DeleteCouponAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// X�c th?c m� coupon v?i s? ti?n ??n h�ng
        /// </summary>
        [HttpPost("validate")]
        [Authorize]
        public async Task<IActionResult> ValidateCoupon(ValidateCouponDto validateCouponDto)
        {
            var result = await _couponService.ValidateCouponAsync(validateCouponDto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// �p d?ng coupon v� t�nh to�n gi?m gi�
        /// </summary>
        [HttpPost("apply")]
        [Authorize]
        public async Task<IActionResult> ApplyCoupon(ApplyCouponDto applyCouponDto)
        {
            var result = await _couponService.ApplyCouponAsync(applyCouponDto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// T�nh to�n s? ti?n gi?m gi� c?a coupon
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