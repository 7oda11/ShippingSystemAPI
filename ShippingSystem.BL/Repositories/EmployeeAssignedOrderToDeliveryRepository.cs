using Microsoft.EntityFrameworkCore;
using ShippingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.BL.Repositories
{
    public class EmployeeAssignedOrderToDeliveryRepository: GenericRepository<Core.Entities.EmployeeAssignOrderToDelivery>, Core.Interfaces.IEmployeeAssignedOrderToDeliveryRepository
    {
        public EmployeeAssignedOrderToDeliveryRepository(ShippingContext context) : base(context)
        {


        }
        public async Task<EmployeeAssignOrderToDelivery?> FindAssignmentByDeliveryAndOrderId(int deliveryManId, int orderId)
        {
            return await _context.EmployeeAssignOrderToDeliveries
                .Include(a => a.Order)
                .FirstOrDefaultAsync(a => a.DeliveryID == deliveryManId && a.OrderID == orderId);
        }

        public async Task<List<EmployeeAssignOrderToDelivery>> GetByDeliveryManId(int deliveryManId)
        {
            return await _context.EmployeeAssignOrderToDeliveries
                .Where(x => x.DeliveryID == deliveryManId)
                .ToListAsync();
        }

    }
}
