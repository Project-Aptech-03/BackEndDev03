using AutoMapper;
using ProjectDemoWebApi.DTOs.Auth;
using ProjectDemoWebApi.DTOs.Category;
using ProjectDemoWebApi.DTOs.Coupon;
using ProjectDemoWebApi.DTOs.CustomerAddress;
using ProjectDemoWebApi.DTOs.Manufacturer;
using ProjectDemoWebApi.DTOs.Order;
using ProjectDemoWebApi.DTOs.Payment;
using ProjectDemoWebApi.DTOs.Products;
using ProjectDemoWebApi.DTOs.Publisher;
using ProjectDemoWebApi.DTOs.ShoppingCart;
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

            // Manufacturer mappings
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
            CreateMap<Products, ProductsResponseDto>();
            CreateMap<CreateProductsDto, Products>()
                   .ForMember(dest => dest.ProductPhotos, opt => opt.Ignore());


            // Shopping Cart mappings
            CreateMap<AddToCartDto, ShoppingCart>();
            CreateMap<UpdateCartItemDto, ShoppingCart>();
            CreateMap<ShoppingCart, ShoppingCartResponseDto>();

            // Customer Address mappings
            CreateMap<CreateCustomerAddressDto, CustomerAddresses>();
            CreateMap<UpdateCustomerAddressDto, CustomerAddresses>();
            CreateMap<CustomerAddresses, CustomerAddressResponseDto>();

            // Order mappings
            CreateMap<CreateOrderFromCartDto, Orders>();
            CreateMap<UpdateOrderDto, Orders>();
            CreateMap<Orders, OrderResponseDto>()
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer));
            CreateMap<OrderItems, OrderItemResponseDto>();

            // Payment mappings
            CreateMap<CreatePaymentDto, Payments>();
            CreateMap<UpdatePaymentDto, Payments>();
            CreateMap<Payments, PaymentResponseDto>();

            // Coupon mappings
            CreateMap<CreateCouponDto, Coupons>();
            CreateMap<UpdateCouponDto, Coupons>();
            CreateMap<Coupons, CouponResponseDto>();
        }
    }
}
