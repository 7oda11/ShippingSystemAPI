using AutoMapper;
using ShippingSystem.Core.DTO;
using ShippingSystem.Core.Entities;

namespace ShippingSystem.API.Mapping
{
    public class MappConfig : Profile
    {
        public MappConfig()
        {
            CreateMap<ShippingType, ShippingTypeDTO>().ReverseMap();
            CreateMap<WeightSetting, WeightSettingDTO>().ReverseMap();
        }
    }
}
