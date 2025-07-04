using AutoMapper;
using ShippingSystem.Core.DTO.Order;
using ShippingSystem.Core.Entities;

namespace ShippingSystem.API.Mapping
{
    public class ProductMapping:Profile
    {
        public ProductMapping()
        {
            CreateMap<AddOrderItemDTO, Product>().AfterMap((src, des)=>{
                des.Name=src.ProductName;
                des.Quantity=src.Quantity;
                des.Weight = src.Weight;
                des.Price=src.Price;

            });
        }
    }
}
