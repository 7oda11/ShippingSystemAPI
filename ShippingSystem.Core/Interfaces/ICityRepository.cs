
using Microsoft.AspNetCore.Components.Server.Circuits;
using ShippingSystem.Core.DTO.City;
using ShippingSystem.Core.Entities;



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Interfaces
{
    public interface ICityRepository: IGenericRepository<City>
    {
        Task<IEnumerable<City>> GetCitiesWithGovernmentsNameAsync();
        Task<City> GetCityWithGovernmentByIdAsync(int id);
        Task <City> GetCityWithName(string name);
        Task<IEnumerable<CityNameDTO>> GetAllCitiesWithGovId(int govId);    
    }
}
