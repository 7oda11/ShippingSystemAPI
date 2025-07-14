using Microsoft.EntityFrameworkCore;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.BL.Repositories
{
    public class DeliveryManRepository: GenericRepository<Core.Entities.DeliveryMan>, IDeliveryManRepository
    {
        public DeliveryManRepository(ShippingContext context) : base(context)
        {
        }
        public async Task<DeliveryMan> FindByUserIdAsync(string userId)
        {
            return await _context.DeliveryMen
                .Include(dm => dm.User)
                .FirstOrDefaultAsync(dm => dm.UserId == userId);
        }

        public async Task<IEnumerable<DeliveryMan>> GetByCityId(int cityID)
        {
          return await _context.DeliveryMen.Where(dm=>dm.CityID == cityID)
              .ToListAsync();
        }



        public async Task<List<DeliveryMan>> GetAllAsync()
        {
            return await _context.DeliveryMen.ToListAsync();
        }


        public async Task<int> Count()
        {
            return await _context.DeliveryMen.CountAsync();
        }

        public async Task<DeliveryMan> FindByNameAsync(string name)
        {
            return await _context.DeliveryMen
                .FirstOrDefaultAsync(dm => dm.Name.Contains(name));
        }

    }

}
