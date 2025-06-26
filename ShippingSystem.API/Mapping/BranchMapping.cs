using AutoMapper;
using ShippingSystem.Core.DTO.Branch;
using ShippingSystem.Core.Entities;

namespace ShippingSystem.API.Mapping
{
    public class BranchMapping: Profile
    {
        public BranchMapping()
        {

            CreateMap<Branch, BranchDTO>().ReverseMap();
           // CreateMap<Branch, BranchDTO>()
           //.ForMember(dest => dest.Employees, opt => opt.MapFrom(src => src.Employees));
            
        }
    
    
    }
}
