using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShippingSystem.Core.DTO;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;

namespace ShippingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GroupController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateGroupDTO dto)
        {
            var group = _mapper.Map<Group>(dto);

            await _unitOfWork.GroupRepository.Add(group);
            await _unitOfWork.SaveAsync();

            if (dto.PermissionIds?.Any() == true)
            {
                foreach (var pid in dto.PermissionIds)
                {
                    var groupPermission = new GroupPermission
                    {
                        GroupId = group.Id,
                        PermissionId = pid
                    };

                    _unitOfWork.GroupPermissionRepository.AddGroupPermission(groupPermission);
                }

                await _unitOfWork.SaveAsync();
            }

            var loadedGroup = await _unitOfWork.GroupRepository.GetGroupWithPermissions(group.Id);
            var result = _mapper.Map<GroupDetailsDTO>(loadedGroup);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var groups = await _unitOfWork.GroupRepository
                .GetAllIncluding(g => g.GroupPermissions, g => g.Employees);

            var result = _mapper.Map<List<GroupDetailsDTO>>(groups);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var group = await _unitOfWork.GroupRepository
                .GetByIdIncluding(id, g => g.GroupPermissions, g => g.Employees);

            if (group == null)
                return NotFound();

            var result = _mapper.Map<GroupDetailsDTO>(group);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateGroupDTO dto)
        {
            var group = await _unitOfWork.GroupRepository.GetById(id);
            if (group == null)
                return NotFound();

            group.Name = dto.Name;

            var oldPermissions = _unitOfWork.GroupPermissionRepository.GetByGroupId(id);
            _unitOfWork.GroupPermissionRepository.RemoveGroupPermissions(oldPermissions);

            if (dto.PermissionIds?.Any() == true)
            {
                foreach (var pid in dto.PermissionIds)
                {
                    var newPermission = new GroupPermission
                    {
                        GroupId = id,
                        PermissionId = pid
                    };
                    _unitOfWork.GroupPermissionRepository.AddGroupPermission(newPermission);
                }
            }

            await _unitOfWork.SaveAsync();
            var loadedGroup = await _unitOfWork.GroupRepository.GetGroupWithPermissions(group.Id);
            var result = _mapper.Map<GroupDetailsDTO>(loadedGroup);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var group = await _unitOfWork.GroupRepository.GetById(id);
            if (group == null)
                return NotFound();

            var oldPermissions = _unitOfWork.GroupPermissionRepository.GetByGroupId(id);
            _unitOfWork.GroupPermissionRepository.RemoveGroupPermissions(oldPermissions);

            await _unitOfWork.GroupRepository.Delete(group);
            await _unitOfWork.SaveAsync();

            return Ok("Deleted");
        }
    }
}
