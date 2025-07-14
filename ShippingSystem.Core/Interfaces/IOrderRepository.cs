using ShippingSystem.Core.DTO;
using ShippingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {

        Task<bool> HasOrdersForVendorAsync(int vendorId);

        Task<IEnumerable<Order>> GetOrdersByVendorId(string userId);
        Task<IEnumerable<Order>> GetOrderAssignedToDeliveryMan(int deliveryManId);

        Task<Order?> GetByIdWithProducts(int id);


        Task<IEnumerable<Order>> GetAllWithVendorNames();

        Task<Order> GetOrderByID(int orderId);
        Task<IQueryable<Order>> GetQueryable();

        Task<List<Order>> GetOrdersByIdsAsync(List<int> orderIds);

        Task<List<Order>> GetByDateRangeAsync(DateTime start, DateTime end);

        Task<List<Order>> GetAllOrdersAsync();
        Task<Order> GetLastOrderInCityAsync(int cityId);
        Task<List<Order>> GetCancelledOrdersAsync(DateTime startDate, DateTime endDate);
        Task<List<Order>> GetOrdersByCityAsync(int cityId);

        Task<List<Order>> GetByDateRangeWithDetailsAsync(
            DateTime start,
            DateTime end,
            bool includeAssignments = false,
            bool includeDeliveryMan = false,
            bool includeCancellations = false);

        Task<List<Order>> GetByCityAndDateRangeWithDetailsAsync(
            int cityId,
            DateTime start,
            DateTime end,
            bool includeAssignments = false,
            bool includeDeliveryMan = false);
    }

}

