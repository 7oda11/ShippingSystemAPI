using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.DTO
{
    public class GroupDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> PermissionNames { get; set; }
    }
}
