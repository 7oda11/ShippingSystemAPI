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
        private readonly ShippingContext context;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config, ShippingContext context)
        {
            _userManager = userManager;
            _config = config;
            this.context = context;
        }
        private string GenerateJwtToken(ApplicationUser user, IList<string> roles, int? employeeId)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var claims = new List<Claim>
                            {
                               //new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                               new Claim("id",user.Id.ToString()),
                               new Claim("role",roles.First()),
                               new Claim(JwtRegisteredClaimNames.Email, user.Email),
                               new Claim(ClaimTypes.Name, user.UserName),
                                //new Claim("EmployeeId", user.Id.ToString())


                            };
            if (employeeId.HasValue)
            {
                claims.Add(new Claim("EmployeeId", employeeId.Value.ToString()));
            }

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

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
        public async Task<IActionResult> Login( LoginDTO loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Unauthorized("Invalid username or password");
            }

            var roles = await _userManager.GetRolesAsync(user);
            //find emp that attached to user
            var employeeId= await context.Employees.FirstOrDefaultAsync(e=>e.UserId == user.Id);

            var token = GenerateJwtToken(user, roles, employeeId?.Id);

            return Ok(new
            {
                token,
                expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:DurationInMinutes"])),
                user = new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    roles
                }
            });
        }


    }
}
