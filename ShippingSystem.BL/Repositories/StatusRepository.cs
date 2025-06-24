using ShippingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.BL.Repositories
{
    public class StatusRepository: GenericRepository<Core.Entities.Status>, Core.Interfaces.IStatusRepository
    {
        public StatusRepository(ShippingContext context) : base(context)
        { }
    }
}
