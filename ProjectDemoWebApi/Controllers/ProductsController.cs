using Google.Apis.Storage.v1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.DTOs.Product;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Services.Interface;

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
    CancellationToken cancellationToken)
        {
            var existing = await _productService.GetProductByIdAsync(id, cancellationToken);
            if (existing == null)
                return NotFound();

            // Cập nhật thông tin cơ bản
            existing.Name = request.Name;
            existing.Price = request.Price;
            existing.Description = request.Description;

            var existingImageIds = request.ExistingImageIds ?? new List<int>();

            // Xoá ảnh cũ không còn tồn tại trong request
            var imagesToRemove = existing.ProductImages
                .Where(img => !existingImageIds.Contains(img.Id))
                .ToList();

            foreach (var img in imagesToRemove)
            {
                await _googleCloudStorageService.DeleteFileAsync(img.ImageUrl);
                existing.ProductImages.Remove(img);
            }

            // Upload ảnh mới nếu có
            if (request.NewImages != null && request.NewImages.Any())
            {
                var uploadedUrls = await _googleCloudStorageService.UploadFilesAsync(request.NewImages, "products", cancellationToken);

                foreach (var url in uploadedUrls)
                {
                    existing.ProductImages.Add(new ProductImage
                    {
                        ProductId = existing.Id,
                        ImageUrl = url,
                        IsMain = false,
                        OrderIndex = existing.ProductImages.Count // hoặc tính toán thứ tự
                    });
                }
            }

            await _productService.UpdateProductAsync(existing, cancellationToken);

            return Ok(existing);
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
