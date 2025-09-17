using AutoMapper;
using ProjectDemoWebApi.DTOs.Auth;
using ProjectDemoWebApi.DTOs.Blog;
using ProjectDemoWebApi.DTOs.Category;
using ProjectDemoWebApi.DTOs.Coupon;
using ProjectDemoWebApi.DTOs.CustomerAddress;
using ProjectDemoWebApi.DTOs.Manufacturer;
using ProjectDemoWebApi.DTOs.Order;
using ProjectDemoWebApi.DTOs.Products;
using ProjectDemoWebApi.DTOs.Publisher;
using ProjectDemoWebApi.DTOs.ShoppingCart;
using ProjectDemoWebApi.DTOs.SubCategory;
using ProjectDemoWebApi.DTOs.User;
using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Auth mappings
            CreateMap<RegisterRequest, Users>();

            // User mappings
            CreateMap<CreateUserRequestDto, Users>();
            CreateMap<Users, UsersResponseDto>();
            CreateMap<UpdateUserDto, Users>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            //profile
            CreateMap<Users, ProfileResponseDto>();
            CreateMap<ProfileUpdateDto, Users>();


            // Category mappings
            CreateMap<CreateCategoryDto, Categories>();
            CreateMap<UpdateCategoryDto, Categories>();
            CreateMap<Categories, CategoryResponseDto>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));
            CreateMap<CreateCategoryDto, Categories>()
                .ForMember(dest => dest.SubCategories, opt => opt.Ignore());
            CreateMap<SubCategories, SubCategoryResponseDto>();

            CreateMap<Categories, CategoryResponseDto>()
                .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories));

            // SubCategory mappings
            CreateMap<SubCategories, SubCategoryResponseDto>();
            CreateMap<Categories, CategoryResponseDto>()
            .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count))
            .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories));

            CreateMap<CreateCategoryDto, Categories>()
             .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src =>
                 src.SubCategories != null
                     ? src.SubCategories.Select(sub => new SubCategories
                     {
                         SubCategoryName = sub.SubCategoryName.Trim(),
                         IsActive = true,
                         CreatedDate = DateTime.UtcNow
                     }).ToList()
                     : new List<SubCategories>()
             ));

            CreateMap<UpdateCategoryDto, Categories>()
             .ForMember(dest => dest.SubCategories, opt => opt.Ignore()); 

            CreateMap<Categories, CategoryResponseDto>()
             .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count))
             .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories)); 
            
            CreateMap<CreateManufacturerDto, Manufacturers>();
            CreateMap<UpdateManufacturerDto, Manufacturers>();
            CreateMap<Manufacturers, ManufacturerResponseDto>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));

            // Publisher mappings
            CreateMap<CreatePublisherDto, Publishers>();
            CreateMap<UpdatePublisherDto, Publishers>();
            CreateMap<Publishers, PublisherResponseDto>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));

            // Products mappings
            CreateMap<CreateProductsDto, Products>();
            CreateMap<UpdateProductsDto, Products>();
            CreateMap<Products, ProductsResponseDto>()
            .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.ProductPhotos));

            CreateMap<ProductPhotos, ProductPhotoResponseDto>();



            // Shopping Cart mappings
            CreateMap<AddToCartDto, ShoppingCart>();
            CreateMap<UpdateCartItemDto, ShoppingCart>();
            CreateMap<ShoppingCart, ShoppingCartResponseDto>();

            // Customer Address mappings
            CreateMap<CreateCustomerAddressDto, CustomerAddresses>();
            CreateMap<UpdateCustomerAddressDto, CustomerAddresses>();
            CreateMap<CustomerAddresses, CustomerAddressResponseDto>();

            // Order mappings
            CreateMap<CreateOrderDto, Orders>();
            CreateMap<UpdateOrderDto, Orders>();
            CreateMap<Orders, OrderResponseDto>()
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer));
            CreateMap<OrderItems, OrderItemResponseDto>();

            // Coupon mappings
            CreateMap<CreateCouponDto, Coupons>();
            CreateMap<UpdateCouponDto, Coupons>();
            CreateMap<Coupons, CouponResponseDto>();

            //=============faq===============
            //CreateMap<Faq, FaqDto>();
            //CreateMap<CreateFaqDto, Faq>();
            //CreateMap<UpdateFaqDto, Faq>();

            CreateMap<CreateBlogDto, Blogs>();
            CreateMap<UpdateBlogDto, Blogs>();
            CreateMap<Blogs, BlogResponseDto>();
            CreateMap<Blogs, BlogListResponseDto>();
            CreateMap<CreateCommentDto, BlogComments>();
            CreateMap<UpdateCommentDto, BlogComments>();
            CreateMap<BlogComments, CommentResponseDto>();
        }
    }
}
