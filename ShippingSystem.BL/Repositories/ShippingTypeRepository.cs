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
        private readonly ShippingContext context;
        public ShippingTypeRepository(ShippingContext context) : base(context)
        {
            this.context = context;
        }

        public void UpdateAsync(ShippingType type)
        {
            context.Entry(type).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

        }
    }
}
