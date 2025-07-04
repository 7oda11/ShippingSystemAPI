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

            CreateMap<CreateGroupDTO, Group>();
            CreateMap<UpdateGroupDTO, Group>();
            CreateMap<Group, GroupDetailsDTO>()
               .ForMember(dest => dest.PermissionNames,
               opt => opt.MapFrom(src => src.GroupPermissions.Select(gp => gp.Permission.Name).ToList()));
 


            CreateMap<Permission, PermissionDTO>().ReverseMap();
            CreateMap<CreatePermissionDTO, Permission>();
            CreateMap<UpdatePermissionDTO, Permission>();
        }
    }
}
