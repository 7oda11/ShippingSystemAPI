using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.DTO
{
    public class UpdateDeliveryManDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public int CityId { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
