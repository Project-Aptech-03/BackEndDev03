using DotNetEnv;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Services
{
    public class GoogleCloudStorageService : IGoogleCloudStorageService
    {
        private readonly IConfiguration _config;
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;
        private readonly IWebHostEnvironment _env;


        public GoogleCloudStorageService(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env; 
            string rootPath = _env.ContentRootPath;

            string credentialFileName = _config["GoogleCloud:CredentialPath"];
            string credentialPath = Path.Combine(rootPath, credentialFileName);

            if (!File.Exists(credentialPath))
            {
                throw new FileNotFoundException(
                    $"Google credential file not found at: {credentialPath}." +
                    "\nEnsure the file exists and GoogleCloud:CredentialPath in appsettings.json is correct.");
            }

            _storageClient = StorageClient.Create(GoogleCredential.FromFile(credentialPath));
            _bucketName = config["GoogleCloud:BucketName"];
        }

        public async Task<List<string>> UploadFilesAsync(List<IFormFile> files, string folderName, CancellationToken cancellationToken = default)
        {
            var uploadedUrls = new List<string>();

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                {
                    continue; 
                }
                var objectName = $"{folderName}/{Guid.NewGuid()}_{file.FileName}";

                using var stream = file.OpenReadStream();

                await _storageClient.UploadObjectAsync(
                    _bucketName,
                    objectName,
                    file.ContentType,
                    stream,
                    cancellationToken: cancellationToken
                );

                var url = $"https://storage.googleapis.com/{_bucketName}/{objectName}";
                uploadedUrls.Add(url);
            }

            return uploadedUrls;
        }

        public async Task<string> UploadFileMainAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file");

            var objectName = $"{folderName}/{Guid.NewGuid()}_{file.FileName}";

            using var stream = file.OpenReadStream();

            await _storageClient.UploadObjectAsync(
                _bucketName,
                objectName,
                file.ContentType,
                stream,
                cancellationToken: cancellationToken
            );

            var url = $"https://storage.googleapis.com/{_bucketName}/{objectName}";
            return url;
        }


        public async Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(fileUrl))
                throw new ArgumentException("File URL cannot be null or empty.", nameof(fileUrl));

            try
            {
                // Parse object name from public URL variants
                // e.g. https://storage.googleapis.com/{bucket}/{object}
                // or   https://{bucket}.storage.googleapis.com/{object}
                var uri = new Uri(fileUrl);
                var absolutePath = uri.AbsolutePath.TrimStart('/');

                // Ensure URL refers to our bucket
                if (!fileUrl.Contains(_bucketName, StringComparison.Ordinal))
                    throw new InvalidOperationException("Invalid bucket in file URL.");

                // Remove bucket prefix from path if present
                string objectName = absolutePath.StartsWith(_bucketName + "/", StringComparison.Ordinal)
                    ? absolutePath.Substring(_bucketName.Length + 1)
                    : absolutePath;

                await _storageClient.DeleteObjectAsync(_bucketName, objectName, cancellationToken: cancellationToken);
            }
            catch (Google.GoogleApiException e) when (e.Error.Code == 404)
            {
                // File không tồn tại, có thể đã bị xóa rồi
                Console.WriteLine($"[DeleteFileAsync] File not found: {fileUrl}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DeleteFileAsync] Unexpected error: {ex.Message}");
                throw;
            }
        }


    }
}
