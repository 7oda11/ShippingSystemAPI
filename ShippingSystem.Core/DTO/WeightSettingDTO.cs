using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.DTO
{
    public record WeightSettingDTO
    {
        public int Id { get; set; }
        public string WeightRange { get; set; }
        public double ExtraPrice { get; set; }
    }
}
