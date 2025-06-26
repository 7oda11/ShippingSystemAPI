
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IBranchRepository BranchRepository { get; }
        ICityRepository CityRepository { get; }
        IDeliveryManRepository DeliveryManRepository { get; }
        IEmployeeAssignedOrderToDeliveryRepository EmployeeAssignedOrderToDeliveryRepository { get; }
        IOrderRepository OrderRepository { get; }
        IEmployeeRepository EmployeeRepository { get; }
        IGovernmentRepository GovernmentRepository { get; }
        IOrderCustomerPhonesRepository OrderCustomerPhonesRepository { get; }
        IProductRepository ProductRepository { get; }
        IShippingTypeRepository ShippingTypeRepository { get; }
        IStatusRepository StatusRepository { get; }
        IWeightSettingRepository WeightSettingRepository { get; }
        IVendorPhonesRepository VendorPhonesRepository { get; }
        IVendorRepository VendorRepository { get; }

        void Save();
        Task SaveAsync(); // Optional if you support async operationsW



    }
}
