using Microsoft.EntityFrameworkCore;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;
using System.Linq.Expressions;

namespace ShippingSystem.BL.Repositories
{
    public class GroupRepository : GenericRepository<Group>, IGroupRepository
    {
        private readonly ShippingContext _context;

        public GroupRepository(ShippingContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Group>> GetAllIncluding(params Expression<Func<Group, object>>[] includes)
        {
            IQueryable<Group> query = _context.Groups;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public async Task<Group?> GetByIdIncluding(int id, params Expression<Func<Group, object>>[] includes)
        {
            IQueryable<Group> query = _context.Groups;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Group?> GetGroupWithPermissions(int id)
        {
            return await _context.Groups
                .Include(g => g.GroupPermissions)
                .ThenInclude(gp => gp.Permission)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

    }
}
