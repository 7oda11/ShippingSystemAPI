
using ShippingSystem.Core.Entities;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Interfaces
{
    public interface IEmployeeAssignedOrderToDeliveryRepository : IGenericRepository<EmployeeAssignOrderToDelivery>
    {
        Task<EmployeeAssignOrderToDelivery?> FindAssignmentByDeliveryAndOrderId(int deliveryManId, int orderId);

        Task<List<EmployeeAssignOrderToDelivery>> GetByDeliveryManId(int deliveryManId);


    }
}
