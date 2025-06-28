using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    }

}
