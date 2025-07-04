using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.DTO.Order
{
    public class UpdateOrderDTO
    {
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; }

        [Required]
        [Phone]
        public string CustomerPhone1 { get; set; }

        [Phone]
        public string CustomerPhone2 { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int GovernmentId { get; set; }

        [Required]
        public int CityId { get; set; }
        [Required]

        public string VillageName { get; set; }
        [Required]

        public bool IsShippedToVillage { get; set; }

        [Required]
        public int ShippingTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string VendorName { get; set; }
        [Required]

        [StringLength(250)]
        public string VendorAddress { get; set; }
        [Required]

        public int StatusId { get; set; }
        public int? VendorId { get; set; }
        [Required]

        [Range(0, double.MaxValue, ErrorMessage = "TotalPrice must be a positive number.")]
        public decimal TotalPrice { get; set; }

        public string Notes { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "TotalWeight must be a positive number.")]
        public double TotalWeight { get; set; }

        [MinLength(1, ErrorMessage = "At least one order item is required.")]
        public List<AddOrderItemDTO> OrderItems { get; set; } = new List<AddOrderItemDTO>();
    }
}
