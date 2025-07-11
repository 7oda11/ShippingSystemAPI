using AutoMapper;
using ShippingSystem.API.LookUps;
using ShippingSystem.Core.DTO.Order;
using ShippingSystem.Core.Entities;

namespace ShippingSystem.API.Mapping
{
    public class OrderMapping : Profile
    {
        public OrderMapping()
        {
            CreateMap<Order, OrderDTO>().AfterMap((src, dest) =>
            {
                dest.Id = src.Id.ToString();
                dest.Date = src.CreationDate;
                dest.CustomerName = src.CustomerName;
                dest.CustomerPhone = src.CustomerPhone1;
                dest.Governmennt = src.City?.Government?.Name ?? string.Empty;
                dest.City = src.City?.Name ?? string.Empty;
                dest.TotalPrice = (decimal)src.TotalCost;
                dest.status = src.Status.Name;
            }).ReverseMap();

            CreateMap<AddOrderDTO, Order>()
                 .ForMember(dest => dest.Status, opt => opt.Ignore())
                .AfterMap((src, dest) =>
            {
                dest.CustomerName = src.CustomerName;
                dest.CustomerPhone1 = src.CustomerPhone1;
                dest.CustomerPhone2 = src.CustomerPhone2;
                dest.EmailAddress = src.Email;
                dest.CityId = src.CityId;
                dest.IsShippedToVillage = src.IsShippedToVillage;
                dest.VendorId = src.VendorId;
                dest.ShippingTypeId = src.ShippingTypeId;
                dest.TotalCost = (double)src.TotalPrice;
                dest.Notes = src.Notes;
                dest.TotalWeight = src.TotalWeight;
                dest.CreationDate = DateTime.Now;
                dest.OrderType = "Normal"; // Assuming a default value for OrderType
                dest.PaymentType = "Cash"; // Assuming a default value for PaymentType
                //dest.StatusId =(int) OrderStatus.Pending;

            }).ReverseMap();
            //CreateMap<Order, AllOrderDTO>();
            CreateMap<AllOrderDTO, Order>().ForMember(dest => dest.Status, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.Id = src.Id;
                    dest.CustomerName = src.CustomerName;
                    dest.CustomerPhone1 = src.CustomerPhone1;
                    dest.CustomerPhone2 = src.CustomerPhone2;
                    dest.EmailAddress = src.Email;
                    dest.Address = src.Address;
                    dest.CityId = src.CityId;
                    dest.IsShippedToVillage = src.IsShippedToVillage;
                    dest.VendorId = src.VendorId;
                    dest.ShippingTypeId = src.ShippingTypeId;
                    dest.TotalCost = (double)src.TotalPrice;
                    dest.Notes = src.Notes;
                    dest.TotalWeight = src.TotalWeight;
                    dest.CreationDate = DateTime.Now;
                    dest.OrderType = "Normal"; // Assuming a default value for OrderType
                    dest.PaymentType = "Cash";

                }).ReverseMap();


            CreateMap<UpdateOrderDTO, Order>()
    .ForMember(dest => dest.Products, opt => opt.Ignore()); // هنمسكه يدويًا

            CreateMap<AddOrderItemDTO, Product>();
        }
    }
}
