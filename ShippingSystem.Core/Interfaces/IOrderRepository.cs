using ShippingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {

        Task<bool> HasOrdersForVendorAsync(int vendorId);

        Task<IEnumerable<Order>> GetOrdersByVendorId(string userId);
        Task<IEnumerable<Order>> GetOrderAssignedToDeliveryMan(int deliveryManId);
        Task<Order?> GetByIdWithProducts(int id);

    }
}
