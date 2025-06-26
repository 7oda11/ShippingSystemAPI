using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Entities
{
    public class ShippingType
    {
        [Key]
        public int Id { get; set; }
        public string ShippingTypeName { get; set; }
        public decimal ShippingPrice { get; set; }

    }
}
