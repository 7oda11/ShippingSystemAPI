using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Entities
{
    public class OrderCancellation
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public string Reason { get; set; } // Note from delivery man

        public DateTime CancelledAt { get; set; } = DateTime.UtcNow;
    }
}
