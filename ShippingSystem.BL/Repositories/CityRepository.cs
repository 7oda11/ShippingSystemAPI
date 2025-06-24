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
         public CityRepository(ShippingContext context) : base(context)
        {
        }
    }
}
