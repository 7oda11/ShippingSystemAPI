using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShippingSystem.Core.DTO;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;
using System.Security.Claims;

namespace ShippingSystem.API.Controllers
{
    [Authorize(Roles = "DeliveryMan")]
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryStatisticsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeliveryStatisticsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("order-status-count")]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var dm = await _unitOfWork.DeliveryManRepository.FindByUserIdAsync(userId);
            if (dm == null) return Unauthorized();

            var assignments = await _unitOfWork.EmployeeAssignedOrderToDeliveryRepository.GetAll();
            var orders = assignments
                .Where(a => a.DeliveryID == dm.Id)
                .Select(a => a.Order)
                .Distinct();

            var stats = orders
                .GroupBy(o => o.Status.Name)
                .Select(g => new { Status = g.Key, Count = g.Count() });
            return Ok(stats);
        }

        [HttpPut("change-status")]
        public async Task<IActionResult> ChangeOrderStatusByDelivery(UpdateOrderStatusByDeliveryDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var dm = await _unitOfWork.DeliveryManRepository.FindByUserIdAsync(userId);
            if (dm == null) return Unauthorized();

            var assignment = (await _unitOfWork.EmployeeAssignedOrderToDeliveryRepository.GetAll())
                .FirstOrDefault(a => a.DeliveryID == dm.Id && a.OrderID == dto.OrderId);

            if (assignment == null) return NotFound("This order is not assigned to you.");

            var order = assignment.Order;
            if (order == null) return NotFound("Order not found.");

            var newStatus = await _unitOfWork.StatusRepository.GetById(dto.NewStatusId);
            if (newStatus == null) return BadRequest("Invalid status.");

            order.StatusId = newStatus.Id;

            if (newStatus.Name.ToLower() == "cancelled")
            {
                if (string.IsNullOrWhiteSpace(dto.CancellationNote))
                    return BadRequest("Cancellation note is required when cancelling an order.");

                var cancellation = new OrderCancellation
                {
                    OrderId = order.Id,
                    Reason = dto.CancellationNote
                };

                await _unitOfWork.OrderCancellationRepository.Add(cancellation);
            }

            await _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveAsync();

            return Ok("Order status updated successfully.");
        }


    }

}
