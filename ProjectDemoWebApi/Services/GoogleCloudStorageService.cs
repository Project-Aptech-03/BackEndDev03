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
            _env = env; // Khởi tạo biến env

            // Lấy đường dẫn thư mục gốc
            string rootPath = _env.ContentRootPath; // Sửa thành _env

            // Lấy tên file từ cấu hình
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

        public async Task<string> UploadFileAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty", nameof(file));
            }

            var objectName = $"{folderName}/{Guid.NewGuid()}_{file.FileName}";

            using var stream = file.OpenReadStream();

            var dataObject = await _storageClient.UploadObjectAsync(
                _bucketName,
                objectName,
                file.ContentType,
                stream,
                cancellationToken: cancellationToken
            );

            // KHÔNG gọi UpdateObjectAsync vì bucket đang bật Uniform bucket-level access
            return $"https://storage.googleapis.com/{_bucketName}/{objectName}";
        }

    }
}
