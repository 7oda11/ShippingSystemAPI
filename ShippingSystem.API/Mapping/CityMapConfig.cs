using AutoMapper;
using ShippingSystem.Core.DTO.City;
using ShippingSystem.Core.Entities;

namespace ShippingSystem.API.Mapping
{
    public class CityMapConfig :Profile
    {
        public CityMapConfig()
        {
            CreateMap<City, CityDTO>().
                ForMember(des => des.GovName,
                opt => opt.MapFrom(src => src.Government.Name))
                .ReverseMap();


        }
    }
}
