using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.DTO
{
    public class UpdateGroupDTO
    {
        public string Name { get; set; }
        public List<int> PermissionIds { get; set; }
    }
}
