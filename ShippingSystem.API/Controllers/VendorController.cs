﻿using Microsoft.AspNetCore.Http;
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

            // Load associated ApplicationUser
            var user = await _userManager.FindByIdAsync(vendor.UserId);
            if (user == null) return NotFound("Associated user not found.");

          await  _unitOfWork.VendorRepository.Delete(vendor);
          await  _unitOfWork.SaveAsync();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok();
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

    }
}
