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




    }
}
