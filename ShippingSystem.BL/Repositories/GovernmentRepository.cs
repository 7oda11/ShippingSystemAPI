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


    public class GovernmentRepository : GenericRepository<Government>, IGovernmentRepository


    {
        public GovernmentRepository(ShippingContext context) : base(context)
        {
        }




        public async Task<IEnumerable<Government>> GetGovernmentsWithCitiesAsync()
        {
            return await context.Governments.Include(g => g.Cities)
                                             .ToListAsync();
        }

        public async Task<Government> GetGovernmentWithCitiesByIdAsync(int id)
        {
            return await context.Governments
                .Include(g => g.Cities)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Government> GetGovernmentByNameAsync(string name)
        {
            return await context.Governments.Include(g => g.Cities).FirstOrDefaultAsync(g => g.Name == name);
        }

       

      
     


    }
}
