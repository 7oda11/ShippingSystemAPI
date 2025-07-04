using ShippingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Interfaces
{
    public interface IGroupRepository : IGenericRepository<Group> 
    {
        Task<List<Group>> GetAllIncluding(params Expression<Func<Group, object>>[] includes);
        Task<Group?> GetByIdIncluding(int id, params Expression<Func<Group, object>>[] includes);
        Task<Group?> GetGroupWithPermissions(int id);

    }

}
