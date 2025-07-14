using Microsoft.EntityFrameworkCore;
using ShippingSystem.Core.DTO.City;
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

        public async Task<IEnumerable<CityNameDTO>> GetAllCitiesWithGovId(int govId)
        {
          return await _context.Cities.Where(c=>c.GovernmentId == govId)
                .Select(c=> new CityNameDTO
                {
                    Name = c.Name,
                    Id = c.Id
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<City>> GetCitiesWithGovernmentsNameAsync()
        {
          return await _context.Cities.Include(c=>c.Government).ToListAsync();
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

        public async Task<City> FindByNameAsync(string name)
        {
            return await _context.Cities
                .FirstOrDefaultAsync(c => c.Name.Contains(name));
        }




    }
}
