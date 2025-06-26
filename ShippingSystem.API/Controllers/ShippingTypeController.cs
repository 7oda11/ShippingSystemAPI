using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShippingSystem.Core.DTO;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShippingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingTypeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ShippingTypeController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var types = await _unitOfWork.ShippingTypeRepository.GetAllAsync();
            var result = _mapper.Map<List<ShippingTypeDTO>>(types);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var type = await _unitOfWork.ShippingTypeRepository.GetByIdAsync(id);
            if (type == null) return NotFound();
            return Ok(_mapper.Map<ShippingTypeDTO>(type));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShippingTypeDTO dto)
        {
            var entity = _mapper.Map<ShippingType>(dto);
            await _unitOfWork.ShippingTypeRepository.AddAsync(entity);
            await _unitOfWork.SaveAsync();
            return Ok(_mapper.Map<ShippingTypeDTO>(entity));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ShippingTypeDTO dto)
        {
            var entity = await _unitOfWork.ShippingTypeRepository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            entity.ShippingTypeName = dto.ShippingTypeName;
            entity.ShippingPrice = dto.ShippingPrice;

            _unitOfWork.ShippingTypeRepository.Update(entity);
            await _unitOfWork.SaveAsync();

            return Ok(_mapper.Map<ShippingTypeDTO>(entity));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _unitOfWork.ShippingTypeRepository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            _unitOfWork.ShippingTypeRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
            return Ok();
        }
    }
}
