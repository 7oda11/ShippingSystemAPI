using Microsoft.EntityFrameworkCore;
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
        private readonly ShippingContext context;

        public OrderRepository(ShippingContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<bool> HasOrdersForVendorAsync(int vendorId)
        {
          return  await   context.Orders.AnyAsync(o=>o.VendorId == vendorId);
        }

        //public async Task<bool> HasOrdersForVendorAsync(int vendorId)
    }
}
