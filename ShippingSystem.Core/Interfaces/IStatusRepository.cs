using ShippingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Interfaces
{
    public interface IStatusRepository:IGenricRepository<ShippingSystem.Core.Entities.Status>
    {
        Task<IEnumerable<Status>> GetAllAsync();
        Task<Status> GetByIdAsync(int id);
        Task<Status> AddAsync(Status status);
        Task UpdateAsync(Status status);
        Task DeleteAsync(Status status);
    }
}
