using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShippingSystem.Core.DTO;
using ShippingSystem.Core.DTO.Branch;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShippingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeightSettingController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WeightSettingController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _unitOfWork.WeightSettingRepository.GetAll();
            var result = _mapper.Map<List<WeightSettingDTO>>(list);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var setting = await _unitOfWork.WeightSettingRepository.GetById(id);
            if (setting == null) return NotFound();
            return Ok(_mapper.Map<WeightSettingDTO>(setting));
        }
        [HttpPost("add-weightsetting")]
        public async Task<IActionResult> Create([FromBody] WeightSettingDTO dto)
        {
            var entity = _mapper.Map<WeightSetting>(dto);
            await _unitOfWork.WeightSettingRepository.Add(entity);
            await _unitOfWork.SaveAsync();
            return Ok(_mapper.Map<WeightSettingDTO>(entity));
        }

        [HttpPut("Update-WeightSettings/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] WeightSettingDTO dto)
        {
            var entity = await _unitOfWork.WeightSettingRepository.GetById(id);
            if (entity == null) return NotFound();

            entity.WeightRange = dto.WeightRange;
            entity.ExtraPrice = dto.ExtraPrice;

            await _unitOfWork.WeightSettingRepository.Update(entity); // ✅ await here!

            return Ok(_mapper.Map<WeightSettingDTO>(entity));
        }



        [HttpDelete("Delete-WeightSetting/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _unitOfWork.WeightSettingRepository.GetById(id);
            if (entity == null) return NotFound();

            _unitOfWork.WeightSettingRepository.Delete(entity);
            //await _unitOfWork.SaveAsync();
            return NoContent();
        }
    }
}
