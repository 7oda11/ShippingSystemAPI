using System.Collections.Generic;

namespace ShippingSystem.Core.DTO
{
    // تمت إزالة الطبقة الخارجية ووضع الأنواع مباشرة في النيم سبيس
    public enum QueryType
    {
        GeneralConversation,
        Summary,
        IndividualPerformance,
        Comparison,
        OrderHistory,
        CancellationReport,
        DeliveryManStats
    }

    public class QueryAnalysisResult
    {
        public QueryType QueryType { get; set; }
        public List<string> TargetEntities { get; set; } = new List<string>();
        public List<string> Metrics { get; set; } = new List<string>();
        public bool IsGeneralConversation { get; set; }
        public string Language { get; set; } = "en";
    }

    // تم نقل تعريفات الـ DTOs إلى هنا
    public class DeliveryPerformanceDTO
    {
        public int DeliveryManId { get; set; }
        public string DeliveryManName { get; set; }
        public int AssignedCount { get; set; }
        public int DeliveredCount { get; set; }
        public int CancelledCount { get; set; }
        public int ReturnedCount { get; set; }
        public string MostFrequentCancellationReason { get; set; }
        public List<string> CancellationReasons { get; set; }
        public double SuccessRate { get; set; }
    }

    public class ComparisonDTO
    {
        public string EntityName { get; set; }
        public int DeliveredCount { get; set; }
        public int CancelledCount { get; set; }
        public double SuccessRate { get; set; }
    }

    public class PerformanceSummaryDTO
    {
        public int TotalOrders { get; set; }
        public int DeliveredCount { get; set; }
        public int CancelledCount { get; set; }
        public double SuccessRate { get; set; }
    }
}