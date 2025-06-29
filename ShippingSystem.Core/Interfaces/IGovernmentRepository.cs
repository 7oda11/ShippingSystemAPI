
using ShippingSystem.Core.Entities;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShippingSystem.Core.Entities;

namespace ShippingSystem.Core.Interfaces
{
    public interface IGovernmentRepository: IGenericRepository<Government>
    {
        Task <IEnumerable<Government>> GetGovernmentsWithCitiesAsync();
        Task<Government> GetGovernmentWithCitiesByIdAsync(int id);
        Task<Government> GetGovernmentByNameAsync(string name);
    }
}
