using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.DTO
{
    public record ShippingTypeDTO
    {
        public int Id { get; set; }
        public string ShippingTypeName { get; set; }
        public decimal ShippingPrice { get; set; }
    }
}
