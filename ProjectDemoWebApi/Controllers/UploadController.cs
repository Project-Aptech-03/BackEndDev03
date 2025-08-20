using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectDemoWebApi.Services;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IGoogleCloudStorageService _storageService;

        public UploadController(IGoogleCloudStorageService storageService)
        {
            _storageService = storageService;
        }
        //[HttpPost]
        //public async Task<IActionResult> UploadImage(IFormFile file, CancellationToken cancellationToken)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("No file uploaded");

        //    var url = await _storageService.UploadFileAsync(file, "products", cancellationToken);
        //    return Ok(new { imageUrl = url });
        //}

    }
}