using ShippingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.BL.Repositories
{
    public class OrderCustomerPhonesRepository: GenericRepository<Core.Entities.OrderCustomerPhones>, Core.Interfaces.IOrderCustomerPhonesRepository
    {
        public OrderCustomerPhonesRepository(ShippingContext context) : base(context)

        { }
    }
}
