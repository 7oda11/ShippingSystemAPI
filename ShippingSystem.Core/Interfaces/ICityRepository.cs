//using ShippingSystem.BL.Repositories;
using ShippingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Interfaces
{
    public interface ICityRepository:IGenricRepository<City>
    {
        Task<IEnumerable<City>> GetCitiesWithGovernmentsNameAsync();
        Task<City> GetCityWithGovernmentByIdAsync(int id);
        Task <City> GetCityWithName(string name);
    }
}
