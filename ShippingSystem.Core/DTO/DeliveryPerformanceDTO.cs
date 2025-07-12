using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.DTO
{
    public class DeliveryPerformanceDTO
    {
        public int DeliveryManId { get; set; }
        public string DeliveryManName { get; set; }
        public int DeliveredCount { get; set; }
        public int CancelledCount { get; set; }
        public int ReturnedCount { get; set; }
        public int AssignedCount { get; set; }
        public List<string> CancellationReasons { get; set; }
        public string MostFrequentCancellationReason { get; set; }
    }
}
