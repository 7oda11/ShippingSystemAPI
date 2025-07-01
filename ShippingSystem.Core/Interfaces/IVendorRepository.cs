using Microsoft.AspNetCore.Mvc;
using ShippingSystem.Core.DTO.Vendor;
using ShippingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Interfaces
{
    public interface IVendorRepository: IGenericRepository<Vendor>
    {
        Task<bool> AddNewVendor(AddVendorDTO vdto);
    }
}
