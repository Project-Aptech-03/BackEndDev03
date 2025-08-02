using AutoMapper;
using ProjectDemoWebApi.DTOs.Auth;
using ProjectDemoWebApi.DTOs.Product;
using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterRequest, Users>();

            // products
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();
            CreateMap<Product, UpdateProductDto>();
        }
    }
}
