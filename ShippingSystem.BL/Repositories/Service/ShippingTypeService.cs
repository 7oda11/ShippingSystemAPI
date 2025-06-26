using AutoMapper;
using ShippingSystem.Core.DTO;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;
using ShippingSystem.Core.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShippingSystem.BL.Services
{
    public class ShippingTypeService : IShippingTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ShippingTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ShippingTypeDTO>> GetAllAsync()
        {
            var types = await _unitOfWork.ShippingTypeRepository.GetAllAsync();
            return _mapper.Map<List<ShippingTypeDTO>>(types);
        }

        public async Task<ShippingTypeDTO> GetByIdAsync(int id)
        {
            var type = await _unitOfWork.ShippingTypeRepository.GetByIdAsync(id);
            return _mapper.Map<ShippingTypeDTO>(type);
        }

        public async Task<ShippingTypeDTO> CreateAsync(ShippingTypeDTO dto)
        {
            var entity = _mapper.Map<ShippingType>(dto);
            await _unitOfWork.ShippingTypeRepository.AddAsync(entity);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<ShippingTypeDTO>(entity);
        }

        public async Task<ShippingTypeDTO> UpdateAsync(int id, ShippingTypeDTO dto)
        {
            var entity = await _unitOfWork.ShippingTypeRepository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.ShippingTypeName = dto.ShippingTypeName;
            entity.ShippingPrice = dto.ShippingPrice;

            _unitOfWork.ShippingTypeRepository.Update(entity);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<ShippingTypeDTO>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.ShippingTypeRepository.GetByIdAsync(id);
            if (entity == null) return false;

            _unitOfWork.ShippingTypeRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
