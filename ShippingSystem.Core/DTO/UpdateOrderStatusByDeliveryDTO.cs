using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.DTO
{
    public class UpdateOrderStatusByDeliveryDTO
    {
        public int OrderId { get; set; }
        public int NewStatusId { get; set; }
    }
}
