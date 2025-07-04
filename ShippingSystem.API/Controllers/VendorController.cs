using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShippingSystem.Core.DTO;
using ShippingSystem.Core.DTO.Vendor;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;

namespace ShippingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public VendorController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateVendorDTO model)
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

                await _userManager.AddToRoleAsync(user, "Vendor");

                var vendor = new Vendor
                {
                    UserId = user.Id,
                    Name = model.FullName,
                    Address = model.Address,
                    Email = model.Email
                };
              await  _unitOfWork.VendorRepository.Add(vendor);
              await  _unitOfWork.SaveAsync();

                return Ok("Vendor Created");
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [HttpPut]
        public async Task< IActionResult> Update(UpdateVendorDTO vm)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var vendor = await _unitOfWork.VendorRepository.GetById(vm.Id);
            if (vendor == null) return NotFound();

            vendor.Name = vm.Name;
            vendor.Email = vm.Email;
            vendor.Address = vm.Address;

            // Also update related ApplicationUser
            if (vendor.User != null)
            {
                vendor.User.FullName = vm.Name;
                vendor.User.Email = vm.Email;
                vendor.User.UserName = vm.Email;
            }

           await _unitOfWork.VendorRepository.Update(vendor);
           await _unitOfWork.SaveAsync();

            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vendors = await _unitOfWork.VendorRepository.GetAll(); // Await the Task to get the result
            var result = vendors.Select(v => new VendorDTO
            {
                Id = v.Id,
                Name = v.Name,
                Email = v.Email,
                Address = v.Address
                
            }).ToList();
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var vendor = await _unitOfWork.VendorRepository.GetById(id);
            if (vendor == null) return NotFound();

            bool hasOrders = await _unitOfWork.OrderRepository.HasOrdersForVendorAsync(vendor.Id);
            if (hasOrders)
            {
                return BadRequest("Can not Delete Vendors with Existing Orders");
            }


            var phones = await _unitOfWork.VendorPhonesRepository.GetPhonesByVendorId(vendor.Id);
            foreach (var phone in phones)
            {
                await _unitOfWork.VendorPhonesRepository.Delete(phone);
            }

            await _unitOfWork.SaveAsync();
            // Load associated ApplicationUser
            var user = await _userManager.FindByIdAsync(vendor.UserId);
            if (user == null) return NotFound("Associated user not found.");


            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Any())
            {
                await _userManager.RemoveFromRoleAsync(user, userRoles.First());

            }
          
          await  _unitOfWork.VendorRepository.Delete(vendor);


            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors);
            await _unitOfWork.SaveAsync();
            return Ok(new {message="Deleted Successfully"});
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var vendor =await _unitOfWork.VendorRepository.GetById(id);
            if (vendor == null) return NotFound();
            var result = new VendorDTO
            {
                Id = vendor.Id,
                Name = vendor.Name,
                Email = vendor.Email,
                Address = vendor.Address
            };
            return Ok(result);
        }


        [HttpPost("add-new-vendor-with-details")]
        public async  Task<IActionResult> addVendorDetails(AddVendorDTO vdto)
        {
            if (vdto == null) return BadRequest("Invalid Data");
           var newVendor = await _unitOfWork.VendorRepository.AddNewVendor(vdto);
            if (!newVendor) return BadRequest("Failed To Add  Vendor");
            return Ok(vdto);
        }

        [HttpPut("Update-Vendor-Details/{id}")]
        public async  Task<IActionResult> UpdateVendorDetails(int id , AddVendorDTO vdto )
        {
            var vendor = await _unitOfWork.VendorRepository.GetById(id);
            if (vendor == null) return BadRequest("This Vendor Not Found");
   
            
               vendor.Name = vdto.name;
                vendor.Email = vdto.email;
            vendor.Address = vdto.address;
                vendor.GovernmentId = vdto.GovernmentId;
            vendor.CityId = vdto.CityId;
            if(vendor.User!= null)
            {
                vendor.User.FullName = vdto.name;
                vendor.User.Email = vdto.email;
                vendor.User.UserName = vdto.email;
                var token = await _userManager.GeneratePasswordResetTokenAsync(vendor.User);
                var newPass = await _userManager.ResetPasswordAsync(vendor.User, token, vdto.password);
                await  _unitOfWork.VendorRepository.Update(vendor);
                await  _unitOfWork.SaveAsync();


            }

            return Ok(vdto);
        }

    }
}
