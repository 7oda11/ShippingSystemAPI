using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShippingSystem.Core.DTO;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;

namespace ShippingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PermissionController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePermissionDTO dto)
        {
            var permission = _mapper.Map<Permission>(dto);
            await _unitOfWork.PermissionRepository.Add(permission);
            await _unitOfWork.SaveAsync();

            var result = _mapper.Map<PermissionDTO>(permission);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var permissions = await _unitOfWork.PermissionRepository.GetAll();
            var result = _mapper.Map<List<PermissionDTO>>(permissions);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var permission = await _unitOfWork.PermissionRepository.GetById(id);
            if (permission == null) return NotFound();

            var result = _mapper.Map<PermissionDTO>(permission);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePermissionDTO dto)
        {
            var existing = await _unitOfWork.PermissionRepository.GetById(id);
            if (existing == null) return NotFound();

            _mapper.Map(dto, existing);
            await _unitOfWork.PermissionRepository.Update(existing);
            await _unitOfWork.SaveAsync();

            var result = _mapper.Map<PermissionDTO>(existing);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var permission = await _unitOfWork.PermissionRepository.GetById(id);
            if (permission == null) return NotFound();

            await _unitOfWork.PermissionRepository.Delete(permission);
            await _unitOfWork.SaveAsync();

            return Ok("Deleted");
        }
    }
}
