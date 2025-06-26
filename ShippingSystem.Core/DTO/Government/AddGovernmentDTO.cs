using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.DTO.Government
{
    public class AddGovernmentDTO
    {
        public string Name { get; set; }
        public List<string>? ListCities { get; set; }
    }
}
