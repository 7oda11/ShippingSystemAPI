using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Entities
{
    public class Permission
    {
        public int Id { get; set; }

        public string Name { get; set; } 

        public virtual ICollection<GroupPermission> GroupPermissions { get; set; }
    }
}
