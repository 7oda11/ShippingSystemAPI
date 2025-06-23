using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; }

        // Navigation properties
        public virtual Employee EmployeeProfile { get; set; }
        public virtual Vendor VendorProfile { get; set; }
        public virtual DeliveryMan DeliveryManProfile { get; set; }
    }
}
