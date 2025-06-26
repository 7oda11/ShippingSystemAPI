using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShippingSystem.Core.Entities;

namespace ShippingSystem.Core.DTO.Government
{
   public record GovernmentDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public List<string>? ListCities { get; set; }
    }
}
