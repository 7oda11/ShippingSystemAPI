using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShippingSystem.Core.Entities;

namespace ShippingSystem.Core.DTO.Branch
{
    public record BranchDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        //public virtual ICollection<Employee> Employees { get; set; }
    }
}
