using ShippingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.BL.Repositories
{
    public class EmployeeRepository: GenericRepository<Core.Entities.Employee>, Core.Interfaces.IEmployeeRepository
    {
        public EmployeeRepository(ShippingContext context) : base(context)
        { }
    
    }
}
