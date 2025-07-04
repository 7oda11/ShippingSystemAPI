using ShippingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Interfaces
{
    public interface IVendorPhonesRepository: IGenericRepository<VendorPhones>
    {
        Task<List<VendorPhones>> GetPhonesByVendorId(int vendorId);
    }
}
