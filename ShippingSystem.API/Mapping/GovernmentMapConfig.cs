using AutoMapper;
using ShippingSystem.Core.DTO.Government;
using ShippingSystem.Core.Entities;

namespace ShippingSystem.API.Mapping
{
    public class GovernmentMapConfig :Profile
    {
        public GovernmentMapConfig()
        {
            CreateMap<Government, GovernmentDTO>()
                .ForMember(des => des.ListCities, opt => opt.MapFrom(src => src.Cities.Select(g => g.Name).ToList())).
                ReverseMap();
            // map addgovdto
            CreateMap<AddGovernmentDTO, Government>()
            .ForMember(des => des.Cities, opt => opt.MapFrom(src => src.ListCities.Select(name => new City { Name = name }).ToList()));




        }
    }
}
