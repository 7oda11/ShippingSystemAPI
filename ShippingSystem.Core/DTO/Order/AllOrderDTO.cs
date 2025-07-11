using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.DTO.Order
{
    public class AllOrderDTO
    {
          public int Id { get; set; }
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
        public string Address { get; set; }
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

        public int? VendorId { get; set; }
        public int? StatusId { get; set; }
        [Required]

        [Range(0, double.MaxValue, ErrorMessage = "TotalPrice must be a positive number.")]
        public decimal TotalPrice { get; set; }

        public string Notes { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "TotalWeight must be a positive number.")]
        public double TotalWeight { get; set; }

        [MinLength(1, ErrorMessage = "At least one order item is required.")]
        public List<loadOrderItemDTO> OrderItems { get; set; } = new List<loadOrderItemDTO>();
    }

    public class loadOrderItemDTO
    {
        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public double Quantity { get; set; }
        [Required]

        [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be greater than zero.")]
        public double Weight { get; set; } // Corrected casing from "weigth"
        [Required]

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal Price { get; set; }

    }
}
