using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Entities
{
    public class Vendor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }


        public string Email { get; set; }

        public float CancelledOrderPercentage { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<VendorPhones> Phones { get; set; }
    }
}
