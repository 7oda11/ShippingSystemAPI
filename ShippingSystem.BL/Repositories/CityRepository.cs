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
    public class CityRepository:GenericRepository<City>, ICityRepository
    {
        private readonly ShippingContext context;

        public CityRepository(ShippingContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<City>> GetCitiesWithGovernmentsNameAsync()
        {
            return await context.Cities.Include(c => c.Government)
                .ToListAsync();
        }

        public async Task<City> GetCityWithGovernmentByIdAsync(int id)
        {
            return await context.Cities
                .Include(c => c.Government)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<City> GetCityWithName(string name)
        {
            return await context.Cities.Include(c => c.Government)
             .FirstOrDefaultAsync(c => c.Name == name);
        }

       
    }
}
