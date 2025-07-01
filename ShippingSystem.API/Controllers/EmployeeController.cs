using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShippingSystem.Core.DTO;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;

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
              await  _unitOfWork.EmployeeRepository.Add(employee);
             await   _unitOfWork.SaveAsync();

                return Ok("Employee Created");
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [HttpPut]
        public async Task<IActionResult> Update(UpdateEmployeeDTO vm)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var employee =await _unitOfWork.EmployeeRepository.GetById(vm.Id);
            if (employee == null) return NotFound();

            employee.BranchId = vm.BranchId;

            if (employee.User != null)
            {
                employee.User.FullName = vm.FullName;
                employee.User.Email = vm.Email;
                employee.User.UserName = vm.Email;
            }

         await   _unitOfWork.EmployeeRepository.Update(employee);
            await _unitOfWork.SaveAsync();

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _unitOfWork.EmployeeRepository.GetAll();
            var result = employees.Select(e => new EmployeeDTO
            {
                Id = e.Id,
                FullName = e.User.FullName,
                Email = e.User.Email,
                BranchName = e.Branch?.Name
            }).ToList();

            if (result == null || !result.Any())
            {
                return BadRequest("Employees not found");
            }
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var emp = await _unitOfWork.EmployeeRepository.GetById(id);
            if (emp == null) return NotFound();

            var user = await _userManager.FindByIdAsync(emp.UserId);
            if (user == null) return NotFound("Associated user not found.");

           await _unitOfWork.EmployeeRepository.Delete(emp);
           await _unitOfWork.SaveAsync();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var emp =await _unitOfWork.EmployeeRepository.GetById(id);
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
