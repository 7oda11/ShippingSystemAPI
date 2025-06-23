using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Entities
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public decimal PickedPrice { get; set; }

        public int GovernmentId { get; set; }

        [ForeignKey("GovernmentId")]
        public virtual Government Government { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<DeliveryMan>  CityDeliveryMen{ get; set; }

    }
}
