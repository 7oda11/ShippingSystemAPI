using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;

namespace ShippingSystem.BL.Repositories
{
    public class GroupPermissionRepository : GenericRepository<GroupPermission>, IGroupPermissionRepository
    {
        private readonly ShippingContext _context;

        public GroupPermissionRepository(ShippingContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<GroupPermission> GetByGroupId(int groupId)
        {
            return _context.GroupPermissions.Where(gp => gp.GroupId == groupId).ToList();
        }

        public void RemoveGroupPermissions(IEnumerable<GroupPermission> groupPermissions)
        {
            _context.GroupPermissions.RemoveRange(groupPermissions);
        }

        public void AddGroupPermission(GroupPermission entity)
        {
            _context.GroupPermissions.Add(entity);
        }
    }
}
