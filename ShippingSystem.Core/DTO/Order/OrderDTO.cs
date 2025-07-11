using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.DTO.Order
{
    public class OrderDTO
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string Governmennt { get; set; } 
        public string City { get; set; }
        public  int CityId{ get; set; }
        public string VendorName { get; set; }
        public string status { get; set; }
        public int statusId { get; set; }
        public decimal TotalPrice { get; set; }
        public string? CancelledOrReturnedNotes { get; set; }


    }
}
