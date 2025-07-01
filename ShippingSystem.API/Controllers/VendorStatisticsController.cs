using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShippingSystem.Core.Interfaces;
using System.Security.Claims;

namespace ShippingSystem.API.Controllers
{
    [Authorize(Roles = "Vendor")]
    [Route("api/[controller]")]
    [ApiController]
    public class VendorStatisticsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public VendorStatisticsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("order-status-count")]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vendor = await _unitOfWork.VendorRepository.FindByUserIdAsync(userId);
            if (vendor == null) return Unauthorized();

            var orders = await _unitOfWork.OrderRepository.GetAll();
            var stats = orders
                .Where(o => o.VendorId == vendor.Id)
                .GroupBy(o => o.Status.Name)
                .Select(g => new { Status = g.Key, Count = g.Count() });
            return Ok(stats);
        }
    }

}
