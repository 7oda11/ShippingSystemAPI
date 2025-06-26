using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Interfaces
{
    public interface IOrderRepository: IGenricRepository<ShippingSystem.Core.Entities.Order>
    {
    }
}
