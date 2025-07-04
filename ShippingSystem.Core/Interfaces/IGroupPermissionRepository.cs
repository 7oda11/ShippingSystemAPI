using ShippingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Interfaces
{
    public interface IGroupPermissionRepository : IGenericRepository<GroupPermission> 
    {
        IEnumerable<GroupPermission> GetByGroupId(int groupId);
        void RemoveGroupPermissions(IEnumerable<GroupPermission> groupPermissions);
        void AddGroupPermission(GroupPermission entity);
    }

}
