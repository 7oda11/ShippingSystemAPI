using Microsoft.EntityFrameworkCore;
using ShippingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.BL.Repositories
{
    public class VendorPhonesRepository : GenericRepository<Core.Entities.VendorPhones>, Core.Interfaces.IVendorPhonesRepository
    {
        public VendorPhonesRepository(ShippingContext context) : base(context)
        { }

        public async Task<List<VendorPhones>> GetPhonesByVendorId(int vendorId)
        {
            return await _context.VendorPhones.Where(p => p.VendorId == vendorId).ToListAsync();
        }
    }
}
