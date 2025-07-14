using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShippingSystem.Core.DTO;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;
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
        private readonly IStatusRepository statusRepository;
        public OrderRepository(ShippingContext context, IStatusRepository statusRepository) : base(context)
        {
            this.context = context;
            this.statusRepository = statusRepository;
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


        public async Task<Order?> GetByIdWithProducts(int id)
        {
            return await context.Orders
                .Include(o => o.Products)
                .Include(o => o.City)
                .Include(o => o.ShippingType)
                .FirstOrDefaultAsync(o => o.Id == id);
        }


        public async Task<IEnumerable<Order>> GetAllWithVendorNames()
        {
            return await context.Orders.Include(o => o.Vendor).Include(c => c.City)
                .ThenInclude(o => o.Government).Include(o => o.Status).ToListAsync();
        }

        public async Task<Order> GetOrderByID(int orderId)
        {
            return await _context.Orders.Include(o =>o.City).ThenInclude(o=>o.Government)
                .Include(o=>o.Vendor)
                .FirstOrDefaultAsync(o=>o.Id==orderId);
        }

        public async Task<IQueryable<Order>> GetQueryable()
        {
            return await Task.FromResult(
                _context.Orders
                    .Include(o => o.Status)
                    .Include(o => o.City)
                    .AsQueryable()
            );

        }

        public async Task<List<Order>> GetOrdersByIdsAsync(List<int> orderIds)
        {
            return await _context.Orders
                .Where(o => orderIds.Contains(o.Id))
                .ToListAsync();
        }

        public async Task<List<Order>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            return await context.Orders
                .Where(o => o.CreationDate >= start && o.CreationDate <= end)
                .Include(o => o.Status)
                .Include(o => o.OrderCancellation)
                .Include(o => o.Assignments)
                    .ThenInclude(a => a.DeliveryMan)
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<Order> GetLastOrderInCityAsync(int cityId)
        {
            return await _context.Orders
                .Where(o => o.CityId == cityId)
                .OrderByDescending(o => o.CreationDate)
                .Include(o => o.Assignments)
                    .ThenInclude(a => a.DeliveryMan)
                .Include(o => o.Status)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Order>> GetCancelledOrdersAsync(DateTime startDate, DateTime endDate)
        {
            var cancelledStatusId = await statusRepository.GetStatusIdByNameAsync("Cancelled");

            return await _context.Orders
                .Where(o => o.StatusId == cancelledStatusId &&
                            o.CreationDate >= startDate &&
                            o.CreationDate <= endDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByCityAsync(int cityId)
        {
            return await _context.Orders
                .Where(o => o.CityId == cityId)
                .Include(o => o.Assignments)
                    .ThenInclude(a => a.DeliveryMan)
                .Include(o => o.Status)
                .Include(o => o.OrderCancellation)
                .ToListAsync();
        }

        public async Task<List<Order>> GetByDateRangeWithDetailsAsync(
      DateTime start,
      DateTime end,
      bool includeAssignments = false,
      bool includeDeliveryMan = false,
      bool includeCancellations = false)
        {
            var query = context.Orders
                .Where(o => o.CreationDate >= start && o.CreationDate <= end)
                .AsQueryable();

            if (includeAssignments)
            {
                query = query.Include(o => o.Assignments);

                if (includeDeliveryMan)
                {
                    query = query.Include(o => o.Assignments)
                                 .ThenInclude(a => a.DeliveryMan);
                }
            }

            if (includeCancellations)
            {
                query = query.Include(o => o.OrderCancellation);
            }

            return await query
                .Include(o => o.Status)
                .ToListAsync();
        }

        public async Task<List<Order>> GetByCityAndDateRangeWithDetailsAsync(
            int cityId,
            DateTime start,
            DateTime end,
            bool includeAssignments = false,
            bool includeDeliveryMan = false)
        {
            var query = context.Orders
                .Where(o => o.CityId == cityId &&
                           o.CreationDate >= start &&
                           o.CreationDate <= end)
                .AsQueryable();

            if (includeAssignments)
            {
                query = query.Include(o => o.Assignments);

                if (includeDeliveryMan)
                {
                    query = query.Include(o => o.Assignments)
                                 .ThenInclude(a => a.DeliveryMan);
                }
            }

            return await query
                .Include(o => o.Status)
                .Include(o => o.OrderCancellation)
                .ToListAsync();
        }

    }
}
