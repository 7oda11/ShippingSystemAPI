using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Entities
{
    public class EmployeeAssignOrderToDelivery
    {
        [Key]
        public int Id { get; set; }
        public int OrderID { get; set; }
        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; }
        public int EmployeeID { get; set; }
        [ForeignKey("EmployeeID")]
        public virtual Employee Employee { get; set; }
        public int DeliveryID { get; set; }
    [ForeignKey("DeliveryID")]
        public virtual DeliveryMan DeliveryMan { get; set; }
      
    }
}
