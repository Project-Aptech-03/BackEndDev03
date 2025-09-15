using ProjectDemoWebApi.Repositories;
using ProjectDemoWebApi.Repositories.Implementations;
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
            services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
            services.AddScoped<ICustomerAddressRepository, CustomerAddressRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IProductReturnRepository, ProductReturnRepository>();
            services.AddScoped<IStockMovementRepository, StockMovementRepository>();
            services.AddScoped<ICouponRepository, CouponRepository>();
            services.AddScoped<ICustomerQueryRepository, CustomerQueryRepository>();
            services.AddScoped<IAdminReplyRepository, AdminReplyRepository>();
            
            services.AddScoped<ISystemSettingsRepository, SystemSettingsRepository>();

            //===faq    =================
            services.AddScoped<IFaqRepository, FaqRepository>();
            services.AddScoped<IFaqService, FaqService>();

            services.AddScoped<IBlogRepository, BlogRepository>();
            services.AddScoped<IBlogCommentRepository, BlogCommentRepository>();
            services.AddScoped<IBlogLikeRepository, BlogLikeRepository>();
            services.AddScoped<ICommentLikeRepository, CommentLikeRepository>();
            services.AddScoped<IAuthorFollowRepository, AuthorFollowRepository>();

            return services;
        }

        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            // Business service registrations
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IManufacturerService, ManufacturerService>();
            services.AddScoped<IProductsService, ProductsService>();
            services.AddScoped<IShoppingCartService, ShoppingCartService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<IPublishersService, PublishersService>();
            // Note: ICustomerAddressService implementation needed

            // Keep existing services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IGoogleCloudStorageService, GoogleCloudStorageService>();
            services.AddScoped<IRoleSeederService, RoleSeederService>();

            // Blog services
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IBlogCommentService, BlogCommentService>();
            services.AddScoped<IAuthorFollowService, AuthorFollowService>();

            // gg
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