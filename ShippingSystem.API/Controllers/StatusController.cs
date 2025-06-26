using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;
using ShippingSystem.Core.DTO.Status;

namespace ShippingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IStatusRepository _statusRepository;

        public StatusController(IStatusRepository statusRepository)
        {
            _statusRepository = statusRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StatusDto>>> GetStatuses()
        {
            var statuses = await _statusRepository.GetAllAsync();

            var result = statuses.Select(s => new StatusDto { Id = s.Id, Name = s.Name });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StatusDto>> GetStatus(int id)
        {
            var status = await _statusRepository.GetByIdAsync(id);

            if (status == null)
                return NotFound();

            return new StatusDto { Id = status.Id, Name = status.Name };
        }

        [HttpPost]
        public async Task<ActionResult<StatusDto>> CreateStatus(CreateStatusDto dto)
        {
            var status = new Status { Name = dto.Name };

            await _statusRepository.AddAsync(status);

            var result = new StatusDto { Id = status.Id, Name = status.Name };

            return CreatedAtAction(nameof(GetStatus), new { id = status.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateStatusDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var status = await _statusRepository.GetByIdAsync(id);

            if (status == null)
                return NotFound();

            status.Name = dto.Name;

            await _statusRepository.UpdateAsync(status);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStatus(int id)
        {
            var status = await _statusRepository.GetByIdAsync(id);

            if (status == null)
                return NotFound();

            await _statusRepository.DeleteAsync(status);

            return NoContent();
        }


    }
}
