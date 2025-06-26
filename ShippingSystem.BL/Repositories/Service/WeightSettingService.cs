using AutoMapper;
using ShippingSystem.Core.DTO;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;
using ShippingSystem.Core.Service;
using ShippingSystem.Core.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShippingSystem.BL.Services
{
    public class WeightSettingService : IWeightSettingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WeightSettingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<WeightSettingDTO>> GetAllAsync()
        {
            var list = await _unitOfWork.WeightSettingRepository.GetAllAsync();
            return _mapper.Map<List<WeightSettingDTO>>(list);
        }

        public async Task<WeightSettingDTO> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.WeightSettingRepository.GetByIdAsync(id);
            return _mapper.Map<WeightSettingDTO>(entity);
        }

        public async Task<WeightSettingDTO> CreateAsync(WeightSettingDTO dto)
        {
            var entity = _mapper.Map<WeightSetting>(dto);
            await _unitOfWork.WeightSettingRepository.AddAsync(entity);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<WeightSettingDTO>(entity);
        }

        public async Task<WeightSettingDTO> UpdateAsync(int id, WeightSettingDTO dto)
        {
            var entity = await _unitOfWork.WeightSettingRepository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.WeightRange = dto.WeightRange;
            entity.ExtraPrice = dto.ExtraPrice;

            _unitOfWork.WeightSettingRepository.Update(entity);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<WeightSettingDTO>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.WeightSettingRepository.GetByIdAsync(id);
            if (entity == null) return false;

            _unitOfWork.WeightSettingRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
