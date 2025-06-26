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
    public class EmployeeController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeController(UserManager<ApplicationUser> userManager,IUnitOfWork unitOfWork)
        {
            this._userManager = userManager;
            this._unitOfWork = unitOfWork;
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDTO model)
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

                await _userManager.AddToRoleAsync(user, "Employee");

                var employee = new Employee { UserId = user.Id, BranchId = model.BranchId };
                _unitOfWork.EmployeeRepository.Add(employee);
                _unitOfWork.Save();

                return Ok("Employee Created");
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [HttpPut]
        public IActionResult Update(UpdateEmployeeDTO vm)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var employee = _unitOfWork.EmployeeRepository.GetById(vm.Id);
            if (employee == null) return NotFound();

            employee.BranchId = vm.BranchId;

            if (employee.User != null)
            {
                employee.User.FullName = vm.FullName;
                employee.User.Email = vm.Email;
                employee.User.UserName = vm.Email;
            }

            _unitOfWork.EmployeeRepository.Update(employee);
            _unitOfWork.Save();

            return Ok();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _unitOfWork.EmployeeRepository.GetAll().Select(e => new EmployeeDTO
            {
                Id = e.Id,
                FullName = e.User.FullName,
                Email = e.User.Email,
                BranchName = e.Branch?.Name
            }).ToList();
            if (result == null)
            {
                return BadRequest("Employees not found");
            }
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var emp = _unitOfWork.EmployeeRepository.GetById(id);
            if (emp == null) return NotFound();

            var user = await _userManager.FindByIdAsync(emp.UserId);
            if (user == null) return NotFound("Associated user not found.");

            _unitOfWork.EmployeeRepository.Delete(emp);
            _unitOfWork.Save();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var emp = _unitOfWork.EmployeeRepository.GetById(id);
            if (emp == null) return NotFound();
            var result = new EmployeeDTO
            {
                Id = emp.Id,
                FullName = emp.User.FullName,
                Email = emp.User.Email,
                BranchName = emp.Branch?.Name
            };
            return Ok(result);
        }

    }
}
