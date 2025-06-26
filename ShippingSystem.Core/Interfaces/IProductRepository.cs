using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Interfaces
{
    public interface IProductRepository: IGenricRepository<ShippingSystem.Core.Entities.Product>
    {
    }
}
