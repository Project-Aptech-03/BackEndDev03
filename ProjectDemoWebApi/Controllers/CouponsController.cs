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
        /// Get a list of all coupons (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCoupons()
        {
            var result = await _couponService.GetAllCouponsAsync();
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Create a new coupon (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCoupon(CreateCouponDto createCouponDto)
        {
            var result = await _couponService.CreateCouponAsync(createCouponDto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Update coupon information (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCoupon(int id, UpdateCouponDto updateCouponDto)
        {
            var result = await _couponService.UpdateCouponAsync(id, updateCouponDto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Delete a coupon (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var result = await _couponService.DeleteCouponAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Apply a coupon and calculate the discount
        /// </summary>
        [HttpPost("apply")]
        [Authorize]
        public async Task<IActionResult> ApplyCoupon(ApplyCouponDto applyCouponDto)
        {
            var result = await _couponService.ApplyCouponAsync(applyCouponDto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Use a coupon upon successful order (reduces the quantity)
        /// </summary>
        [HttpPost("use/{couponCode}")]
        [Authorize]
        public async Task<IActionResult> UseCoupon(string couponCode)
        {
            var result = await _couponService.UseCouponAsync(couponCode);
            return StatusCode(result.StatusCode, result);
        }
    }
}
