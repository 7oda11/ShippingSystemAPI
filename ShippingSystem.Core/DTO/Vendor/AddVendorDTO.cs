using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.DTO.Vendor
{
    public class AddVendorDTO
    {
        public string name { get; set; }
        public string email { get; set; }

        public string address { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
        public int GovernmentId { get; set; }
        public int CityId { get; set; }

    }
}
