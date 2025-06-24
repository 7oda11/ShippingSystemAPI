using ShippingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.BL.Repositories
{
   public class ShippingTypeRepository: GenericRepository<Core.Entities.ShippingType>, Core.Interfaces.IShippingTypeRepository
    {
        public ShippingTypeRepository(ShippingContext context) : base(context)
        {
        }
    }
}
