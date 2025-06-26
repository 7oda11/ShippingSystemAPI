using ShippingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.BL.Repositories
{
    public class OrderRepository : GenericRepository<Core.Entities.Order>, Core.Interfaces.IOrderRepository
    {
        public OrderRepository(ShippingContext context) : base(context)
        {
        }
    }
}
