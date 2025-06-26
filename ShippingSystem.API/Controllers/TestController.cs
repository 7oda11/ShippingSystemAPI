using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShippingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        // ✅ Open to everyone
        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok("✅ Public endpoint - no auth required.");
        }

        // ✅ Requires any authenticated user
        [Authorize]
        [HttpGet("authenticated")]
        public IActionResult Authenticated()
        {
            return Ok($"✅ Authenticated user: {User.Identity.Name}");
        }

        // ✅ Requires Admin role
        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult AdminOnly()
        {
            return Ok("✅ Admin-only endpoint.");
        }

        // ✅ Requires Vendor role
        [Authorize(Roles = "Vendor")]
        [HttpGet("vendor")]
        public IActionResult VendorOnly()
        {
            return Ok("✅ Vendor-only endpoint.");
        }

        // ✅ Requires DeliveryMan role
        [Authorize(Roles = "DeliveryMan")]
        [HttpGet("delivery")]
        public IActionResult DeliveryOnly()
        {
            return Ok("✅ DeliveryMan-only endpoint.");
        }

        // ✅ Requires Employee role
        [Authorize(Roles = "Employee")]
        [HttpGet("employee")]
        public IActionResult EmployeeOnly()
        {
            return Ok("✅ Employee-only endpoint.");
        }
    }
}
