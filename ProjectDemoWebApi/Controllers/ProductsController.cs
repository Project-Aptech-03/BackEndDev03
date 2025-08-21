using Google.Apis.Storage.v1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.DTOs.Product;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Controllers
{
    [Authorize]
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
            var productImage = new List<ProductImage>();
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
        public async Task<IActionResult> Create([FromForm] CreateProductDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.Images == null || request.Images.Count == 0)
                return BadRequest("Phải cung cấp ít nhất một hình ảnh.");

            var productImages = new List<ProductImage>();
            string mainImageUrl = string.Empty;

            for (int i = 0; i < request.Images.Count; i++)
            {
                var formFile = request.Images[i];
                var uploadedUrls = await _googleCloudStorageService.UploadFilesAsync(
                    new List<IFormFile> { formFile },
                    "products");
                var uploadedUrl = uploadedUrls.FirstOrDefault();
                if (string.IsNullOrEmpty(uploadedUrl))
                    return BadRequest("Không thể upload ảnh.");
                var image = new ProductImage
                {
                    ImageUrl = uploadedUrl,
                    IsMain = i == 0,
                    OrderIndex = i
                };
                if (i == 0)
                {
                    mainImageUrl = uploadedUrl;
                }
                productImages.Add(image);
            }
            var product = new Product
            {
                Name = request.Name,
                Price = request.Price,
                Description = request.Description,
                ImageUrl = mainImageUrl,
                ProductImages = productImages
            };
            await _productService.CreateProductAsync(product);
            return Ok(product);
        }

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

            if (request.Name != null)
                existing.Name = request.Name;

            if (request.Description != null)
                existing.Description = request.Description;

            if (request.Price.HasValue)
                existing.Price = request.Price.Value;

            if (image != null)
            {
                if (!string.IsNullOrEmpty(existing.ImageUrl))
                {
                    await _googleCloudStorageService.DeleteFileAsync(existing.ImageUrl, cancellationToken);
                }

                var newImageUrl = await _googleCloudStorageService.UploadFileMainAsync(
                    image,
                    folderName: "products",
                    cancellationToken
                );
                existing.ImageUrl = newImageUrl;
            }

            await _productService.UpdateProductAsync(existing);
            return Ok(new
            {
                message = "Product updated successfully.",
                product = existing
            });
        }

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
