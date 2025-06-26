using ShippingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.BL.Repositories
{
    public class GovernmentRepository: GenericRepository<Core.Entities.Government>, Core.Interfaces.IGovernmentRepository
    {
        public GovernmentRepository(ShippingContext context) : base(context)
        {
        }}
}
