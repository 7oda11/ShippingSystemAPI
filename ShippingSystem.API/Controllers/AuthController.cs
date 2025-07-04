using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShippingSystem.Core.DTO;
using ShippingSystem.Core.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShippingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly ShippingContext _context;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config, ShippingContext context)
        {
            _userManager = userManager;
            _config = config;
            _context = context;
        }

        private string GenerateJwtToken(ApplicationUser user, IList<string> roles, List<string> permissions)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            foreach (var permission in permissions)
                claims.Add(new Claim("Permission", permission));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["DurationInMinutes"])),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                return Unauthorized("Invalid username or password");

            var roles = await _userManager.GetRolesAsync(user);

            // Get group permissions
            var employee = await _context.Employees
                .Include(e => e.Group)
                .ThenInclude(g => g.GroupPermissions)
                .ThenInclude(gp => gp.Permission)
                .FirstOrDefaultAsync(e => e.UserId == user.Id);

            var permissions = employee?.Group?.GroupPermissions?
                .Select(gp => gp.Permission.Name)
                .Distinct()
                .ToList() ?? new List<string>();

            var token = GenerateJwtToken(user, roles, permissions);

            return Ok(new
            {
                token,
                expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:DurationInMinutes"])),
                user = new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    roles,
                    permissions
                }
            });
        }
    }
}
