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
        [HttpPost]
        [Route("multiple")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadFiles([FromForm] List<IFormFile> files, [FromQuery] string folderName, CancellationToken cancellationToken)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("No files uploaded.");
            }
            if (string.IsNullOrWhiteSpace(folderName))
            {
                return BadRequest("Folder name is required.");
            }
            try
            {
                var uploadedUrls = await _storageService.UploadFilesAsync(files, folderName, cancellationToken);
                return Ok(new { Urls = uploadedUrls });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error uploading files: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("single")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromQuery] string folderName, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            if (string.IsNullOrWhiteSpace(folderName))
            {
                return BadRequest("Folder name is required.");
            }
            try
            {
                var uploadedUrl = await _storageService.UploadFileMainAsync(file, folderName, cancellationToken);
                var response = new { Url = uploadedUrl };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error uploading file: {ex.Message}");
            }
        }
    }
}