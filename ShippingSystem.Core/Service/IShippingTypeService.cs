using ShippingSystem.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Service
{
    public interface IShippingTypeService
    {
        Task<List<ShippingTypeDTO>> GetAllAsync();
        Task<ShippingTypeDTO> GetByIdAsync(int id);
        Task<ShippingTypeDTO> CreateAsync(ShippingTypeDTO dto);
        Task<ShippingTypeDTO> UpdateAsync(int id, ShippingTypeDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
