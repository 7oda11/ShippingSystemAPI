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

        public async Task<IEnumerable<Order>> GetOrdersByVendorId(string userId)
        {
            Console.WriteLine("Received UserId: " + userId);
            var vendor = await _context.Vendors.FirstOrDefaultAsync(v=>v.UserId == userId);
            Console.WriteLine("Vendor found? " + (vendor != null));
            if (vendor == null)
            { return new List<Order>(); }


            return await _context.Orders.Where(o=>o.VendorId== vendor.Id).ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrderAssignedToDeliveryMan(int deliveryManId)
        {
          return await _context.Orders.Where(o=>o.Assignments.Any(a=>a.DeliveryID==deliveryManId)).ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllWithVendorNames()
        {
            return await context.Orders.Include(o => o.Vendor).Include(c=> c.City)
                .ThenInclude(o => o.Government).Include(o => o.Status).ToListAsync();
        }
    }
}
