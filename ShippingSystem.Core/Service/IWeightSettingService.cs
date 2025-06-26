using ShippingSystem.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Service
{
    public interface IWeightSettingService
    {
        Task<List<WeightSettingDTO>> GetAllAsync();
        Task<WeightSettingDTO> GetByIdAsync(int id);
        Task<WeightSettingDTO> CreateAsync(WeightSettingDTO dto);
        Task<WeightSettingDTO> UpdateAsync(int id, WeightSettingDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
