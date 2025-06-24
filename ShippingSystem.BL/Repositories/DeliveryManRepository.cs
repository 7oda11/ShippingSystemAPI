using ShippingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.BL.Repositories
{
    public class DeliveryManRepository: GenericRepository<Core.Entities.DeliveryMan>, Core.Interfaces.IDeliveryManRepository
    {
        public DeliveryManRepository(ShippingContext context) : base(context)
        {
        }
    }
  
}
