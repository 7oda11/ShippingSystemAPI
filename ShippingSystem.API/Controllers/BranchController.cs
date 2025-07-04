using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShippingSystem.Core.DTO.Branch;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;

namespace ShippingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        IUnitOfWork _unitOfWork;
        private readonly IMapper mapper;

        public BranchController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllBranches()
        {
            var branches = await _unitOfWork.BranchRepository.GetAll();
            if (branches == null || !branches.Any())
            {
                return NotFound("No branches found.");
            }

            // Map the branches to BranchDTOs using auto mapper
            //var branchDTOs = mapper.Map<List<ShippingSystem.Core.DTO.Branch.BranchDTO>>(branches);
            return Ok(mapper.Map<IEnumerable<BranchDTO>>(branches));
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetBranchById(int id)
        {
            var branch = await _unitOfWork.BranchRepository.GetById(id);
            if (branch == null)
            {
                return NotFound($"Branch with ID {id} not found.");
            }
            // Map the branch to BranchDTO using auto mapper
            return Ok(mapper.Map<BranchDTO>(branch));


        }
        [HttpPost("Add-Branch")]
        public async Task<IActionResult> CreateBranch(BranchDTO branchDTO)
        {
            if (branchDTO == null)
            {
                return BadRequest("Branch data is null.");
            }
            var branch = mapper.Map<Branch>(branchDTO);
            await _unitOfWork.BranchRepository.Add(branch);
            //save changes
            await _unitOfWork.SaveAsync();
            return CreatedAtAction(nameof(GetBranchById), new { id = branch.Id }, mapper.Map<BranchDTO>(branch));
        }


        [HttpPut("Update-Branch/{id}")]
        public async Task<IActionResult> UpdateBranch(int id, BranchDTO branchDTO)
        {
            if (branchDTO == null || id != branchDTO.Id)
            {
                return BadRequest("Branch data is invalid.");
            }

            var existingBranch = await _unitOfWork.BranchRepository.GetById(id);
            if (existingBranch == null)
            {
                return NotFound($"Branch with ID {id} not found.");
            }

            mapper.Map(branchDTO, existingBranch);
            _unitOfWork.BranchRepository.Update(existingBranch);
            //await _unitOfWork.SaveAsync();
            return Ok(mapper.Map<BranchDTO>(existingBranch)); 
        }



        [HttpDelete("Delete-Branch/{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var branch = await _unitOfWork.BranchRepository.GetById(id);
            if (branch == null)
            {
                return NotFound($"Branch with ID {id} not found.");
            }
            await _unitOfWork.BranchRepository.Delete(branch);
            return NoContent();
        }

    }
}