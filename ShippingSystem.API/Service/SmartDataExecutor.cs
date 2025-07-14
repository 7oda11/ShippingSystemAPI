using ShippingSystem.API.LookUps;
using ShippingSystem.Core.DTO;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;
using ShippingSystem.Core.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ShippingSystem.Core.DTO.QueryAnalysisResult;

namespace ShippingSystem.API.Service
{
    public class SmartDataExecutor : ISmartDataExecutor
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DeliveryManPerformanceService _performanceService;

        public SmartDataExecutor(
            IUnitOfWork unitOfWork,
            DeliveryManPerformanceService performanceService)
        {
            _unitOfWork = unitOfWork;
            _performanceService = performanceService;
        }

        public async Task<object> ExecuteQueryAsync(QueryAnalysisResult analysis)
        {
            return analysis.QueryType switch
            {
                QueryType.IndividualPerformance => await GetIndividualPerformanceAsync(analysis),
                QueryType.Comparison => await GetComparisonAsync(analysis),
                QueryType.Summary => await GetSummaryAsync(analysis),
                QueryType.OrderHistory => await GetOrderHistoryAsync(analysis),
                QueryType.CancellationReport => await GetCancellationReportAsync(analysis),
                QueryType.DeliveryManStats => await GetDeliveryManStatsAsync(analysis),
                _ => HandleGeneralConversation(analysis.Language)
            };
        }

        private async Task<List<DeliveryPerformanceDTO>> GetIndividualPerformanceAsync(QueryAnalysisResult analysis)
        {
            var results = new List<DeliveryPerformanceDTO>();

            foreach (var entity in analysis.TargetEntities)
            {
                DeliveryMan deliveryMan = null;
                if (int.TryParse(entity, out int id))
                {
                    deliveryMan = await _unitOfWork.DeliveryManRepository.GetById(id);
                }
                else
                {
                    deliveryMan = await _unitOfWork.DeliveryManRepository.FindByNameAsync(entity);
                }

                if (deliveryMan == null) continue;

                var perf = await _performanceService.GetDeliveryManPerformanceAsync(deliveryMan.Id);
                if (perf != null) results.Add(perf);
            }

            return results;
        }

        private async Task<List<ComparisonDTO>> GetComparisonAsync(QueryAnalysisResult analysis)
        {
            var comparisonData = new List<ComparisonDTO>();
            var allDeliveryMen = await _unitOfWork.DeliveryManRepository.GetAll();

            foreach (var dm in allDeliveryMen)
            {
                var perf = await _performanceService.GetDeliveryManPerformanceAsync(dm.Id);
                if (perf == null) continue;

                comparisonData.Add(new ComparisonDTO
                {
                    EntityName = dm.Name,
                    DeliveredCount = perf.DeliveredCount,
                    CancelledCount = perf.CancelledCount,
                    SuccessRate = CalculateSuccessRate(perf)
                });
            }

            return comparisonData.OrderByDescending(c => c.SuccessRate).ToList();
        }

        private double CalculateSuccessRate(DeliveryPerformanceDTO perf)
        {
            return perf.AssignedCount > 0 ?
                (double)perf.DeliveredCount / perf.AssignedCount * 100 :
                0;
        }

        private async Task<PerformanceSummaryDTO> GetSummaryAsync(QueryAnalysisResult analysis)
        {
            var orders = await _unitOfWork.OrderRepository.GetAllOrdersAsync();
            int deliveredCount = orders.Count(o => o.StatusId == (int)OrderStatus.Delivered);
            int totalOrders = orders.Count;

            return new PerformanceSummaryDTO
            {
                TotalOrders = totalOrders,
                DeliveredCount = deliveredCount,
                CancelledCount = orders.Count(o => o.StatusId == (int)OrderStatus.Cancelled),
                SuccessRate = totalOrders > 0 ?
                    (double)deliveredCount / totalOrders * 100 :
                    0
            };
        }

        private async Task<object> GetOrderHistoryAsync(QueryAnalysisResult analysis)
        {
            // Implement based on your specific requirements
            return new { Message = "Order history feature is under development" };
        }

        private async Task<object> GetCancellationReportAsync(QueryAnalysisResult analysis)
        {
            var orders = await _unitOfWork.OrderRepository.GetAllOrdersAsync();
            var cancelledOrders = orders
                .Where(o => o.StatusId == (int)OrderStatus.Cancelled)
                .ToList();

            return new
            {
                TotalCancelled = cancelledOrders.Count,
                Reasons = cancelledOrders
                    .GroupBy(o => o.OrderCancellation.Reason)
                    .Select(g => new { Reason = g.Key, Count = g.Count() })
            };
        }

        private async Task<object> GetDeliveryManStatsAsync(QueryAnalysisResult analysis)
        {
            return new
            {
                TotalDeliveryMen = await _unitOfWork.DeliveryManRepository.Count(),
                // Add more stats as needed
            };
        }

        private string HandleGeneralConversation(string language)
        {
            return language == "ar" ?
                "مرحبًا! كيف يمكنني مساعدتك في تحليل أداء التوصيل اليوم؟" :
                "Hello! How can I assist you with delivery performance analysis today?";
        }
    }
}