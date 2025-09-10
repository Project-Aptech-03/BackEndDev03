using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectDemoWebApi.DTOs.Products;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;

        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }

   


        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _productsService.GetProductsPagedAsync(page, size);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductsDto createProductDto)
        {
            var result = await _productsService.CreateProductAsync(createProductDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] UpdateProductsDto updateProductDto)
        {
            var result = await _productsService.UpdateProductAsync(id, updateProductDto);
            return StatusCode(result.StatusCode, result);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productsService.DeleteProductAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete]
        [Route("batch")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeleteProducts([FromBody] List<int> ids)
        {
            var result = await _productsService.DeleteProductsAsync(ids);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProduct(int id)
        {
            var result = await _productsService.GetProductByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var result = await _productsService.GetProductsByCategoryAsync(categoryId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("manufacturer/{manufacturerId}")]
        public async Task<IActionResult> GetProductsByManufacturer(int manufacturerId)
        {
            var result = await _productsService.GetProductsByManufacturerAsync(manufacturerId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("low-stock")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetLowStockProducts([FromQuery] int threshold = 10)
        {
            var result = await _productsService.GetLowStockProductsAsync(threshold);
            return StatusCode(result.StatusCode, result);
        }

       

       

        [HttpPut("{id}/stock")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] int newStock)
        {
            var result = await _productsService.UpdateStockAsync(id, newStock);
            return StatusCode(result.StatusCode, result);
        }

        
    }
}