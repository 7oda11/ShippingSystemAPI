using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;
using System;

namespace ShippingSystem.BL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ShippingContext _context;

        private IBranchRepository _branchRepository;
        private ICityRepository _cityRepository;
        private IDeliveryManRepository _deliveryManRepository;
        private IEmployeeAssignedOrderToDeliveryRepository _employeeAssignedOrderToDeliveryRepository;
        private IOrderRepository _orderRepository;
        private IEmployeeRepository _employeeRepository;
        private IGovernmentRepository _governmentRepository;
        private IOrderCustomerPhonesRepository _orderCustomerPhonesRepository;
        private IProductRepository _productRepository;
        private IShippingTypeRepository _shippingTypeRepository;
        private IStatusRepository _statusRepository;
        private IWeightSettingRepository _weightSettingRepository;
        private IVendorPhonesRepository _vendorPhonesRepository;
        private IVendorRepository _vendorRepository;

        public UnitOfWork(ShippingContext context)
        {
            _context = context;
        }

        public IBranchRepository BranchRepository =>
            _branchRepository ??= new BranchRepository(_context);

        public ICityRepository CityRepository =>
            _cityRepository ??= new CityRepository(_context);

        public IDeliveryManRepository DeliveryManRepository =>
            _deliveryManRepository ??= new DeliveryManRepository(_context);

        public IEmployeeAssignedOrderToDeliveryRepository EmployeeAssignedOrderToDeliveryRepository =>
            _employeeAssignedOrderToDeliveryRepository ??= new EmployeeAssignedOrderToDeliveryRepository(_context);

        public IOrderRepository OrderRepository =>
            _orderRepository ??= new OrderRepository(_context);

        public IEmployeeRepository EmployeeRepository =>
            _employeeRepository ??= new EmployeeRepository(_context);

        public IGovernmentRepository GovernmentRepository =>
            _governmentRepository ??= new GovernmentRepository(_context);

        public IOrderCustomerPhonesRepository OrderCustomerPhonesRepository =>
            _orderCustomerPhonesRepository ??= new OrderCustomerPhonesRepository(_context);

        public IProductRepository ProductRepository =>
            _productRepository ??= new ProductRepository(_context);

        public IShippingTypeRepository ShippingTypeRepository =>
            _shippingTypeRepository ??= new ShippingTypeRepository(_context);

        public IStatusRepository StatusRepository =>
            _statusRepository ??= new StatusRepository(_context);

        public IWeightSettingRepository WeightSettingRepository =>
            _weightSettingRepository ??= new WeightSettingRepository(_context);

        public IVendorPhonesRepository VendorPhonesRepository =>
            _vendorPhonesRepository ??= new VendorPhonesRepository(_context);

        public IVendorRepository VendorRepository =>
            _vendorRepository ??= new VendorRepository(_context);

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task SaveAsync()
        {
           await _context.SaveChangesAsync();
        }
    }
}
