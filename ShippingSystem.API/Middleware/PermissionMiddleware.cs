using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShippingSystem.Core.Entities;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShippingSystem.API.Middleware
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;

        public PermissionMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context, ShippingContext db)
        {
            var endpoint = context.GetEndpoint();
            var required = endpoint?.Metadata.GetMetadata<RequiresPermissionAttribute>();
            if (required == null) { await _next(context); return; }

            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) { context.Response.StatusCode = 401; return; }

            var emp = await db.Employees.Include(e => e.Group).FirstOrDefaultAsync(e => e.UserId == userId);
            if (emp?.GroupId == null) { context.Response.StatusCode = 403; return; }

            var hasPermission = await db.GroupPermissions
                .AnyAsync(gp => gp.GroupId == emp.GroupId && gp.Permission.Name == required.PermissionName);

            if (!hasPermission) { context.Response.StatusCode = 403; return; }

            await _next(context);
        }
    }
}
