using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IGoogleCloudStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderName, CancellationToken cancellationToken);
    }
}
