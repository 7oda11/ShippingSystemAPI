using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShippingSystem.Core.Interfaces;

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
    }

}
