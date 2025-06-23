using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Entities
{
    public class WeightSetting
    {
         [Key]
        public int Id { get; set; }

        public string WeightRange { get; set; }

        public double ExtraPrice { get; set; }
    }
}
