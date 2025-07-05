
using ShippingSystem.Core.Entities;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Interfaces
{
    public interface IDeliveryManRepository: IGenericRepository<DeliveryMan>
    {
        Task<DeliveryMan> FindByUserIdAsync(string userId);
        //Task<DeliveryMan> FindByUserIdAsync(string userId);


    }
}
