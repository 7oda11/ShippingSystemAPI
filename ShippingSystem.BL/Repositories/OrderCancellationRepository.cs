using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.BL.Repositories
{
    public class OrderCancellationRepository : GenericRepository<OrderCancellation>, IOrderCancellationRepository
    {
        public OrderCancellationRepository(ShippingContext context) : base(context) { }
    }
}
