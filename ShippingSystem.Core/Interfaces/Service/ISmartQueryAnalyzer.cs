using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShippingSystem.Core.DTO;

namespace ShippingSystem.Core.Interfaces.Service
{
    public interface ISmartQueryAnalyzer
    {
        Task<QueryAnalysisResult> AnalyzeQueryAsync(string question);

    }
}
