using Microsoft.Extensions.Options;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services;
using ProjectDemoWebApi.Services.Implementations;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Repository registrations

            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
            services.AddScoped<IPublisherRepository, PublisherRepository>();
            services.AddScoped<IProductsRepository, ProductsRepository>();
            services.AddScoped<IProductPhotosRepository, ProductPhotosRepository>();
            services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
            services.AddScoped<ICustomerAddressRepository, CustomerAddressRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IProductReturnRepository, ProductReturnRepository>();
            services.AddScoped<IStockMovementRepository, StockMovementRepository>();
            services.AddScoped<ICouponRepository, CouponRepository>();
            services.AddScoped<ICustomerQueryRepository, CustomerQueryRepository>();
            services.AddScoped<IAdminReplyRepository, AdminReplyRepository>();
            services.AddScoped<IFaqRepository, FaqRepository>();
            services.AddScoped<ISystemSettingsRepository, SystemSettingsRepository>();
            services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
            services.AddScoped<IBlogRepository, BlogRepository>();
            services.AddScoped<IBlogCommentRepository, BlogCommentRepository>();
            services.AddScoped<IBlogLikeRepository, BlogLikeRepository>();
            services.AddScoped<ICommentLikeRepository, CommentLikeRepository>();
            services.AddScoped<IAuthorFollowRepository, AuthorFollowRepository>();

            return services;
        }

        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            // Core Auth & User Services
            services.AddScoped<IAdminSeederService, AdminSeederService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IRoleSeederService, RoleSeederService>();

            // Business Domain Services
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IManufacturerService, ManufacturerService>();
            services.AddScoped<IProductsService, ProductsService>();
            services.AddScoped<IShoppingCartService, ShoppingCartService>();
            services.AddScoped<ICustomerAddressService, CustomerAddressService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IPublishersService, PublishersService>();
            services.AddScoped<IBlogCommentService, BlogCommentService>();
            services.AddScoped<IAuthorFollowService, AuthorFollowService>();
            services.AddScoped<IFaqService, FaqService>();


            // Payment Services - Register HttpClient for SePayService
            services.AddHttpClient<ISePayService, SePayService>((serviceProvider, client) =>
            {
                var sePaySettings = serviceProvider.GetRequiredService<IOptions<SePaySettings>>().Value;

                // Check for null safety
                if (string.IsNullOrEmpty(sePaySettings.ApiUrl))
                {
                    throw new InvalidOperationException("SePay ApiUrl is not configured");
                }

                if (string.IsNullOrEmpty(sePaySettings.ApiKey))
                {
                    throw new InvalidOperationException("SePay ApiKey is not configured");
                }

                // Configure base address
                client.BaseAddress = new Uri(sePaySettings.ApiUrl);

                // Configure timeout
                client.Timeout = TimeSpan.FromSeconds(30);

                // Configure headers
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sePaySettings.ApiKey}");
                client.DefaultRequestHeaders.Add("User-Agent", "SePayService/1.0");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "cross-site");
            });

            services.AddHttpClient<ISePayService, SePayService>((serviceProvider, client) =>
            {
                var sePaySettings = serviceProvider.GetRequiredService<IOptions<SePaySettings>>().Value;
                if (string.IsNullOrEmpty(sePaySettings.ApiUrl))
                {
                    throw new InvalidOperationException("SePay ApiUrl is not configured");
                }

                if (string.IsNullOrEmpty(sePaySettings.ApiKey))
                {
                    throw new InvalidOperationException("SePay ApiKey is not configured");
                }

                client.BaseAddress = new Uri(sePaySettings.ApiUrl);

                client.Timeout = TimeSpan.FromSeconds(30);

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sePaySettings.ApiKey}");
                client.DefaultRequestHeaders.Add("User-Agent", "SePayService/1.0");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");

                client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "cross-site");
            });

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IGoogleCloudStorageService, GoogleCloudStorageService>();
            services.AddScoped<IRoleSeederService, RoleSeederService>();
            services.AddScoped<IGoogleCloudStorageService, GoogleCloudStorageService>();
            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddRepositories();
            services.AddBusinessServices();

            return services;
        }
    }
}
