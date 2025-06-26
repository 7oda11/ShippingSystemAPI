using ShippingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.BL.Repositories
{
    public class WeightSettingRepository: GenericRepository<Core.Entities.WeightSetting>, Core.Interfaces.IWeightSettingRepository
    {
        public WeightSettingRepository(ShippingContext context) : base(context)
        { }
    
    }
}
