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
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
    int id,
    [FromForm] UpdateProductDto request,
    [FromForm] List<IFormFile>? newImages,
    CancellationToken cancellationToken)
        {
            var existing = await _productService.GetProductByIdAsync(id, cancellationToken);
            if (existing == null)
                return NotFound();

            // Cập nhật các trường nếu có truyền vào, nếu không thì giữ nguyên
            existing.Name = string.IsNullOrWhiteSpace(request.Name) ? existing.Name : request.Name;
            existing.Price = request.Price == 0 ? existing.Price : request.Price;
            existing.Description = string.IsNullOrWhiteSpace(request.Description) ? existing.Description : request.Description;

            // Nếu có ảnh mới, cập nhật ảnh mới
            if (request.NewImages != null && request.NewImages.Any())
            {
                // Xử lý thêm ảnh mới
                await _productService.ReplaceImagesAsync(existing, request.NewImages, cancellationToken);
            }

            // Nếu có cập nhật ảnh chính
            if (request.MainImageId != null)
            {
                existing.MainImageId = request.MainImageId.Value;
            }

            // Nếu có danh sách ảnh cũ giữ lại
            if (request.ExistingImageIds != null && request.ExistingImageIds.Any())
            {
                await _productService.KeepExistingImagesAsync(existing, request.ExistingImageIds, cancellationToken);
            }

            await _productService.UpdateProductAsync(existing, cancellationToken);
            return Ok(existing);
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
            var mainImageId = request.MainImageId;

            // Xoá ảnh không còn tồn tại trong request
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
                var uploadedUrls = await _googleCloudStorageService.UploadFilesAsync(
                    request.NewImages, "products", cancellationToken);

                foreach (var url in uploadedUrls)
                {
                    var newImage = new ProductImage
                    {
                        ProductId = existing.Id,
                        ImageUrl = url,
                        IsMain = false,
                        OrderIndex = existing.ProductImages.Count
                    };
                    existing.ProductImages.Add(newImage);
                }
            }

            // Cập nhật lại IsMain và OrderIndex
            foreach (var img in existing.ProductImages)
            {
                img.IsMain = mainImageId.HasValue && img.Id == mainImageId;
            }

            // Nếu không có ảnh chính nào được chỉ định, đặt ảnh đầu tiên làm chính
            if (!existing.ProductImages.Any(i => i.IsMain) && existing.ProductImages.Any())
            {
                existing.ProductImages.First().IsMain = true;
            }

            // Cập nhật lại ImageUrl chính của sản phẩm
            var mainImage = existing.ProductImages.FirstOrDefault(i => i.IsMain);
            if (mainImage != null)
            {
                existing.ImageUrl = mainImage.ImageUrl;
            }

            // Cập nhật sản phẩm
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
