using ShippingSystem.Core.Interfaces;
using ShippingSystem.API.LookUps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShippingSystem.Core.DTO;

namespace ShippingSystem.API.Services
{
    public class DeliveryManPerformanceService
    {
        private readonly IUnitOfWork _unit;

        public DeliveryManPerformanceService(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public async Task<DeliveryPerformanceDTO> GetDeliveryManPerformanceAsync(int deliveryManId)
        {
            var deliveryMan = await _unit.DeliveryManRepository.GetById(deliveryManId);
            if (deliveryMan == null) return null;

            var assignments = await _unit.EmployeeAssignedOrderToDeliveryRepository.GetByDeliveryManId(deliveryManId);
            var orderIds = assignments.Select(a => a.OrderID).ToList();

            var orders = await _unit.OrderRepository.GetOrdersByIdsAsync(orderIds);
            var cancellations = await _unit.OrderCancellationRepository.GetReasonsByOrderIds(orderIds);

            var reasonList = cancellations.Select(c => c.Reason).ToList();
            var mostFrequent = reasonList
                .GroupBy(r => r)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? "No data";

            return new DeliveryPerformanceDTO
            {
                DeliveryManId = deliveryManId,
                DeliveryManName = deliveryMan.Name,
                AssignedCount = orders.Count,
                DeliveredCount = orders.Count(o => o.StatusId == (int)OrderStatus.Delivered),
                CancelledCount = orders.Count(o => o.StatusId == (int)OrderStatus.Cancelled),
                ReturnedCount = orders.Count(o => o.StatusId == (int)OrderStatus.Returned),
                CancellationReasons = reasonList,
                MostFrequentCancellationReason = mostFrequent
            };
        }
    }
}