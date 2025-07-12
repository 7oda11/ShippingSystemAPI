using System.Numerics;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShippingSystem.API.Services;
using ShippingSystem.Core.DTO;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;
using ShippingSystem.Core.Interfaces.Service;

namespace ShippingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DeliveryManPerformanceService performanceService;
        private readonly IGPTChatService gptService;

        public EmployeeController(UserManager<ApplicationUser> userManager,IUnitOfWork unitOfWork, DeliveryManPerformanceService performanceService, IGPTChatService gptService)
        {
            this._userManager = userManager;
            this._unitOfWork = unitOfWork;
            this.performanceService = performanceService;
            this.gptService = gptService;
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

                return Ok(new{message = "Employee Created" });
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateEmployeeDTO vm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employee = await _unitOfWork.EmployeeRepository.GetById(vm.Id);
            if (employee == null)
                return NotFound();

            employee.BranchId = vm.BranchId;

            if (employee.User != null)
            {
                employee.User.FullName = vm.FullName;
                employee.User.Email = vm.Email;
                employee.User.UserName = vm.UserName;

                // ✅ فقط غير الباسورد لو تم إرسالها
                if (!string.IsNullOrWhiteSpace(vm.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(employee.User);
                    var newPass = await _userManager.ResetPasswordAsync(employee.User, token, vm.Password);
                    if (!newPass.Succeeded)
                        return BadRequest(newPass.Errors);
                }
            }

            await _unitOfWork.EmployeeRepository.Update(employee);
            await _unitOfWork.SaveAsync();

            return Ok(vm);
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
                BranchName = e.Branch?.Name,
                UserName = e.User.UserName,
                BranchId = e.BranchId


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

            // حذف علاقات المستخدم في AspNetUserRoles
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
            {
                var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, roles);
                if (!removeRolesResult.Succeeded)
                    return BadRequest(removeRolesResult.Errors);
            }

            // حذف الكيان من جدول الموظفين
            await _unitOfWork.EmployeeRepository.Delete(emp);
            await _unitOfWork.SaveAsync();

            // حذف المستخدم نفسه بعد فصل علاقاته
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Deleted Successfully" });
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


        [HttpPost("AskDeliveryBot")]
        public async Task<IActionResult> AskDeliveryBot([FromBody] string question)
        {
            try
            {
                // Handle common questions without OpenAI
                if (question.Contains("how many", StringComparison.OrdinalIgnoreCase) &&
                    question.Contains("delivery men", StringComparison.OrdinalIgnoreCase))
                {
                    var deliveryMenCount = await _unitOfWork.DeliveryManRepository.Count();
                    return Ok(new { answer = $"There are currently {deliveryMenCount} delivery personnel in our system." });
                }

                var allDeliveryMen = await _unitOfWork.DeliveryManRepository.GetAll();
                var promptBuilder = new StringBuilder("Delivery Performance Summary:\n");

                // Only include top 5 performers to reduce token usage
                var topDeliveryMen = allDeliveryMen.Take(5).ToList();

                foreach (var dm in topDeliveryMen)
                {
                    var perf = await performanceService.GetDeliveryManPerformanceAsync(dm.Id);
                    if (perf == null) continue;

                    promptBuilder.AppendLine($@"
                        [ID: {dm.Id}] {dm.Name}
                        - ✓: {perf.DeliveredCount} | ✗: {perf.CancelledCount} | ↩: {perf.ReturnedCount}
                        - Top Cancel: {perf.MostFrequentCancellationReason}
                        ----------------------");
                    }

                promptBuilder.AppendLine($"\nQuestion: {question}");
                promptBuilder.AppendLine("Answer in English based on above data:");

                var response = await gptService.AskAsync(promptBuilder.ToString());
                return Ok(new { answer = response });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

    }
}
