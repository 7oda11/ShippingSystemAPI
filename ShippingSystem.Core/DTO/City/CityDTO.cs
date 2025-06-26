using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.DTO.City
{
    public record CityDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public decimal PickedPrice { get; set; }
        public int GovernmentId { get; set; }
        public string GovName { get; set; }
    }
}
