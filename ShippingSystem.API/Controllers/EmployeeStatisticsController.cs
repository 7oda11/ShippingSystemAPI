using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShippingSystem.Core.DTO;
using ShippingSystem.Core.Interfaces;
using System.Security.Claims;

namespace ShippingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Employee")]
    public class EmployeeStatisticsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeStatisticsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("order-status-count")]
        public async Task<IActionResult> Get()
        {
            var orders = await _unitOfWork.OrderRepository.GetAll();
            var stats = orders
                .GroupBy(o => o.Status.Name)
                .Select(g => new { Status = g.Key, Count = g.Count() });
            return Ok(stats);
        }

        [HttpPut("update-order-status")]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusByDeliveryDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var deliveryMan = await _unitOfWork.DeliveryManRepository.FindByUserIdAsync(userId);
            if (deliveryMan == null) return Unauthorized();

            // validate if order is already assigned to delivery man
            var assignment = await _unitOfWork.EmployeeAssignedOrderToDeliveryRepository
                .FindAssignmentByDeliveryAndOrderId(deliveryMan.Id, dto.OrderId);

            if (assignment == null) return BadRequest("You are not assigned to this order.");

            var order = assignment.Order;
            if (order == null) return NotFound("Order not found.");

            // validate status is found
            var status = await _unitOfWork.StatusRepository.GetById(dto.NewStatusId);
            if (status == null) return NotFound("Status not found.");

            // update status
            order.StatusId = dto.NewStatusId;

            await _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveAsync();

            return Ok("Order status updated.");
        }

    }

}
