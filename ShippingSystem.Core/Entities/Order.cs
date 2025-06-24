using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public string CustomerName { get; set; }

        //public string CustomerPhones { get; set; }

        public string Address { get; set; }

        public string Notes { get; set; }

        public double TotalWeight { get; set; }

        public double TotalCost { get; set; }

        public bool IsShippedToVillage { get; set; }

        public string OrderType { get; set; }

        public string PaymentType { get; set; }

        public DateTime CreationDate { get; set; }

        public int? VendorId { get; set; }

        [ForeignKey("VendorId")]
        public virtual Vendor Vendor { get; set; }


        public int StatusId { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status Status { get; set; }

        public int? CityId { get; set; }

        [ForeignKey("CityId")]
        public virtual City City { get; set; }

        public string ShippingTypeName { get; set; }
        public  decimal ShippingPrice { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public virtual ICollection<EmployeeAssignOrderToDelivery> Assignments { get; set; }
        public virtual ICollection<OrderCustomerPhones> OrderCustomerPhones { get; set; }
    }
}
