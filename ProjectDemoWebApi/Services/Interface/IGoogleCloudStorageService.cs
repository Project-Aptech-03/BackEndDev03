using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IGoogleCloudStorageService
    {
        Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);
        Task<List<string>> UploadFilesAsync(List<IFormFile> files, string folderName, CancellationToken cancellationToken = default);
    }
}
