using Microsoft.AspNetCore.Mvc;
using ProjectDemoWebApi.DTOs.Product;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Services.Interface;
using Microsoft.AspNetCore.Http;

namespace ProjectDemoWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IGoogleCloudStorageService _googleCloudStorageService;

        public ProductsController(
            IProductService productService,
            IGoogleCloudStorageService googleCloudStorageService)
        {
            _productService = productService;
            _googleCloudStorageService = googleCloudStorageService;
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var products = await _productService.GetAllProductsAsync(cancellationToken);
            return Ok(products);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetProductByIdAsync(id, cancellationToken);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromForm] CreateProductDto request,
            [FromForm] IFormFile? image,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string imageUrl = string.Empty;

            if (image != null)
            {
                imageUrl = await _googleCloudStorageService.UploadFileAsync(image, "products", cancellationToken);
            }

            var product = new Product
            {
                Name = request.Name,
                Price = request.Price,
                Description = request.Description,
                ImageUrl = imageUrl
            };

            var created = await _productService.CreateProductAsync(product, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromForm] UpdateProductDto request,
            [FromForm] IFormFile? image,
            CancellationToken cancellationToken)
        {
            var existing = await _productService.GetProductByIdAsync(id, cancellationToken);
            if (existing == null)
                return NotFound();

            string imageUrl = existing.ImageUrl;

            if (image != null)
            {
                imageUrl = await _googleCloudStorageService.UploadFileAsync(image, "products", cancellationToken);
            }

            existing.Name = request.Name;
            existing.Price = request.Price;
            existing.Description = request.Description;
            existing.ImageUrl = imageUrl;

            var updated = await _productService.UpdateProductAsync(existing, cancellationToken);
            return Ok(updated);
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var success = await _productService.DeleteProductAsync(id, cancellationToken);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
