using ShippingSystem.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Interfaces.Service
{
    public interface ISmartDataExecutor
    {
        Task<object> ExecuteQueryAsync(QueryAnalysisResult analysis);

    }
}
