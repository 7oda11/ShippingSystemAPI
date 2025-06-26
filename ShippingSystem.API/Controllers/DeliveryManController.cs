using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShippingSystem.Core.DTO;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;
using ShippingSystem.Core.Migrations;

namespace ShippingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryManController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public DeliveryManController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateDeliveryManDTO model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded) return BadRequest(result.Errors);

                await _userManager.AddToRoleAsync(user, "DeliveryMan");

                var deliveryMan = new DeliveryMan
                {
                    UserId = user.Id,
                    Name = model.FullName,
                    Phone = model.Phone,
                    Email = model.Email,
                    CityID = model.CityId
                };
                _unitOfWork.DeliveryManRepository.Add(deliveryMan);
                _unitOfWork.Save();

                return Ok("DeliveryMan Created");
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [HttpPut]
        public IActionResult Update(UpdateDeliveryManDTO vm)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dm = _unitOfWork.DeliveryManRepository.GetById(vm.Id);
            if (dm == null) return NotFound();

            // Update DeliveryMan fields
            dm.Name = vm.Name;
            dm.Email = vm.Email;
            dm.Phone = vm.Phone;
            dm.CityID = vm.CityId;

            // Update related ApplicationUser fields
            if (dm.User != null)
            {
                dm.User.FullName = vm.Name; // or vm.FullName if provided separately
                dm.User.Email = vm.Email;
                dm.User.UserName = vm.Email; // if using email as username
            }

            _unitOfWork.DeliveryManRepository.Update(dm);
            _unitOfWork.Save();

            return Ok();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _unitOfWork.DeliveryManRepository.GetAll().Select(d => new DeliveryManDTO
            {
                Id = d.Id,
                Name = d.Name,
                Email = d.Email,
                Phone = d.Phone,
                CityName = d.City?.Name
            }).ToList();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var d = _unitOfWork.DeliveryManRepository.GetById(id);
            if (d == null) return NotFound();

            // Load the ApplicationUser (if not already included)
            var user = await _userManager.FindByIdAsync(d.UserId);
            if (user == null) return NotFound("Associated user not found.");

            _unitOfWork.DeliveryManRepository.Delete(d);
            _unitOfWork.Save();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var dm = _unitOfWork.DeliveryManRepository.GetById(id);
            if (dm == null) return NotFound();
            var result = new DeliveryManDTO
            {
                Id = dm.Id,
                Name = dm.Name,
                Email = dm.Email,
                Phone = dm.Phone,
                CityName = dm.City?.Name
            };
            return Ok(result);
        }
    }
}
