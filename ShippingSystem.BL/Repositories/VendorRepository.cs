using ShippingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.BL.Repositories
{
    public class VendorRepository: GenericRepository<Core.Entities.Vendor>, Core.Interfaces.IVendorRepository
    {
        public VendorRepository(ShippingContext context) : base(context)
        { }
    }
}
