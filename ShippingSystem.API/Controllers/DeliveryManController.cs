using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShippingSystem.Core.DTO;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;
using ShippingSystem.Core.Migrations;
using System.Threading.Tasks;

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
               await _unitOfWork.DeliveryManRepository.Add(deliveryMan);
              await  _unitOfWork.SaveAsync();

                return Ok(new { message="DeliveryMan Created" });
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [HttpPut]
        public async Task<IActionResult> Update(UpdateDeliveryManDTO vm)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dm =await  _unitOfWork.DeliveryManRepository.GetById(vm.Id);
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

          await  _unitOfWork.DeliveryManRepository.Update(dm);
          await  _unitOfWork.SaveAsync();

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _unitOfWork.DeliveryManRepository
                .GetAll(); // Fetch the data first

            var deliveryMenWithCities = data
                .Select(d => new
                {
                    DeliveryMan = d,
                    City = _unitOfWork.CityRepository.GetById(d.CityID).Result // Fetch related City data
                })
                .ToList();

            var result = deliveryMenWithCities.Select(d => new DeliveryManDTO
            {
                Id = d.DeliveryMan.Id,
                Name = d.DeliveryMan.Name,
                Email = d.DeliveryMan.Email,
                Phone = d.DeliveryMan.Phone,
                CityName = d.City?.Name
            }).ToList();

            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var d =await _unitOfWork.DeliveryManRepository.GetById(id);
            if (d == null) return NotFound();

            // Load the ApplicationUser (if not already included)
            var user = await _userManager.FindByIdAsync(d.UserId);
            if (user == null) return NotFound("Associated user not found.");

          await  _unitOfWork.DeliveryManRepository.Delete(d);
          await  _unitOfWork.SaveAsync();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var dm =await _unitOfWork.DeliveryManRepository.GetById(id);
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
