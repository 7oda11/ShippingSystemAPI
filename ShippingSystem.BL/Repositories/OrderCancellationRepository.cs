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
    public class OrderCancellationRepository : GenericRepository<OrderCancellation>, IOrderCancellationRepository
    {
        public OrderCancellationRepository(ShippingContext context) : base(context) { }

        public async Task<List<OrderCancellation>> GetReasonsByOrderIds(List<int> orderIds)
        {
            return await _context.Set<OrderCancellation>()
                .Where(c => orderIds.Contains(c.OrderId))
                .ToListAsync();
        }

    }


}
