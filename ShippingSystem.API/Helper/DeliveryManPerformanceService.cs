using ShippingSystem.API.LookUps;
using ShippingSystem.Core.DTO;
using ShippingSystem.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ShippingSystem.Core.DTO.QueryAnalysisResult;

namespace ShippingSystem.API.Service
{
    public class DeliveryManPerformanceService
    {
        private readonly IUnitOfWork _unit;
        private readonly IStatusRepository _statusRepository;

        public DeliveryManPerformanceService(IUnitOfWork unit, IStatusRepository statusRepository)
        {
            _unit = unit;
            _statusRepository = statusRepository;
        }

        public async Task<DeliveryPerformanceDTO> GetDeliveryManPerformanceAsync(int deliveryManId)
        {
            var deliveryMan = await _unit.DeliveryManRepository.GetById(deliveryManId);
            if (deliveryMan == null) return null;

            var assignments = await _unit.EmployeeAssignedOrderToDeliveryRepository
                .GetByDeliveryManId(deliveryManId);

            var orderIds = assignments.Select(a => a.OrderID).ToList();
            var orders = await _unit.OrderRepository.GetOrdersByIdsAsync(orderIds);

            var cancellations = await _unit.OrderCancellationRepository
                .GetReasonsByOrderIds(orderIds);

            // الحصول على معرفات الحالات من الجدول
            var deliveredStatusId = await _statusRepository.GetStatusIdByNameAsync("Delivered");
            var cancelledStatusId = await _statusRepository.GetStatusIdByNameAsync("Cancelled");
            var returnedStatusId = await _statusRepository.GetStatusIdByNameAsync("Returned");

            var deliveredOrders = orders
                .Where(o => o.StatusId == deliveredStatusId)
                .ToList();

            var cancellationReasons = cancellations
                .Select(c => c.Reason)
                .ToList();

            return new DeliveryPerformanceDTO
            {
                DeliveryManId = deliveryManId,
                DeliveryManName = deliveryMan.Name,
                AssignedCount = orders.Count,
                DeliveredCount = deliveredOrders.Count,
                CancelledCount = orders.Count(o => o.StatusId == cancelledStatusId),
                ReturnedCount = orders.Count(o => o.StatusId == returnedStatusId),
                MostFrequentCancellationReason = GetTopCancellationReason(cancellationReasons),
                CancellationReasons = cancellationReasons,
                SuccessRate = orders.Count > 0 ?
                    (double)deliveredOrders.Count / orders.Count * 100 : 0
            };
        }

        private string GetTopCancellationReason(List<string> reasons)
        {
            return reasons
                .GroupBy(r => r)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? "No cancellation data";
        }
    }
}