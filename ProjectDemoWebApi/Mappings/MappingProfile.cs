using AutoMapper;
using ProjectDemoWebApi.DTOs.Request;
using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterRequest, Users>();
        }
    }
}
