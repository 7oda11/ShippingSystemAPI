using Microsoft.EntityFrameworkCore;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ShippingSystem.Core.DTO;
using ShippingSystem.API.Service;

namespace ShippingSystem.API.Service
{
    public class ChatBotService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DeliveryManPerformanceService _performanceService;
        private readonly IStatusRepository _statusRepository;

        public ChatBotService(
            IUnitOfWork unitOfWork,
            DeliveryManPerformanceService performanceService,
            IStatusRepository statusRepository)
        {
            _unitOfWork = unitOfWork;
            _performanceService = performanceService;
            _statusRepository = statusRepository;
        }

        public async Task<string> GetResponseAsync(string question)
        {
            var lowerQuestion = question.ToLower();
            bool isArabic = ContainsArabic(question) ||
                           lowerQuestion.Contains("مرحبا") ||
                           lowerQuestion.Contains("شكرا");

            // الترحيب
            if (lowerQuestion.Contains("hello") || lowerQuestion.Contains("hi") ||
                lowerQuestion.Contains("مرحبا") || lowerQuestion.Contains("اهلا"))
            {
                return isArabic
                    ? "مرحبًا! كيف يمكنني مساعدتك في نظام التوصيل؟"
                    : "Hello! How can I assist you with our delivery system?";
            }

            // المساعدة
            if (lowerQuestion.Contains("help") || lowerQuestion.Contains("مساعدة"))
            {
                return isArabic
                    ? "يمكنني مساعدتك في:\n- عدد الموصلين\n- أداء موصل معين\n- آخر طلب في مدينة\n- تقرير الطلبات الملغاة\n- إحصائيات التوصيل\n- حالة الطلبات اليوم\n- الطلبات المتأخرة\n- أسباب عدم التسليم\n- حالة الطلبات في مدينة محددة"
                    : "I can help you with:\n- Number of delivery men\n- Performance of a specific delivery man\n- Last order in a city\n- Cancelled orders report\n- Delivery statistics\n- Today's orders status\n- Delayed orders\n- Reasons for non-delivery\n- Order status in specific city";
            }

            // عدد الموصلين
            if (lowerQuestion.Contains("how many delivery men") ||
                lowerQuestion.Contains("عدد الموصلين") ||
                lowerQuestion.Contains("كموصلين"))
            {
                var count = await _unitOfWork.DeliveryManRepository.Count();
                return isArabic
                    ? $"يوجد {count} موصل نشط في النظام"
                    : $"There are {count} active delivery men in the system";
            }

            // حالة الطلبات اليوم
            if (lowerQuestion.Contains("today's orders") ||
                lowerQuestion.Contains("orders today") ||
                lowerQuestion.Contains("طلبات اليوم") ||
                lowerQuestion.Contains("اوردرات النهاردة"))
            {
                return await GetTodaysOrdersSummaryAsync(isArabic);
            }

            // تفاصيل الطلبات المتأخرة
            if (lowerQuestion.Contains("pending orders") ||
                lowerQuestion.Contains("delayed orders") ||
                lowerQuestion.Contains("الطلبات المتأخرة") ||
                lowerQuestion.Contains("الاوردرات المتاخرة") ||
                lowerQuestion.Contains("الطلبات اللي لسه ما اتسلمتش") ||
                lowerQuestion.Contains("orders not delivered yet"))
            {
                return await GetPendingOrdersDetailsAsync(isArabic);
            }

            // أسباب عدم التسليم
            if (lowerQuestion.Contains("reasons for delay") ||
                lowerQuestion.Contains("why not delivered") ||
                lowerQuestion.Contains("اسباب التأخير") ||
                lowerQuestion.Contains("ليه مش متسلمة") ||
                lowerQuestion.Contains("اسباب عدم التسليم"))
            {
                return await GetNonDeliveryReasonsAsync(isArabic);
            }

            // حالة طلبات مدينة محددة
            if (lowerQuestion.Contains("order status in") ||
                lowerQuestion.Contains("delay in") ||
                lowerQuestion.Contains("حالة الطلب في") ||
                lowerQuestion.Contains("تأخير في") ||
                lowerQuestion.Contains("اوردرات مدينة"))
            {
                var cityName = ExtractCity(question);
                return await GetCityOrdersStatusAsync(cityName, isArabic);
            }

            // أداء موصل معين
            if (lowerQuestion.Contains("history about") ||
                lowerQuestion.Contains("performance of") ||
                lowerQuestion.Contains("أداء") ||
                lowerQuestion.Contains("تاريخ") ||
                lowerQuestion.Contains("موصل"))
            {
                var deliveryManName = ExtractName(question);
                var deliveryMan = await _unitOfWork.DeliveryManRepository.FindByNameAsync(deliveryManName);

                if (deliveryMan == null)
                {
                    return isArabic
                        ? $"لم يتم العثور على موصل باسم '{deliveryManName}'"
                        : $"Delivery man '{deliveryManName}' not found";
                }

                var perf = await _performanceService.GetDeliveryManPerformanceAsync(deliveryMan.Id);

                return isArabic
                    ? $"أداء الموصل {deliveryMan.Name}:\n" +
                      $"- الطلبات المسندة: {perf.AssignedCount}\n" +
                      $"- تم التوصيل: {perf.DeliveredCount} ({perf.SuccessRate:F1}%)\n" +
                      $"- تم الإلغاء: {perf.CancelledCount}\n" +
                      $"- تم الإرجاع: {perf.ReturnedCount}\n" +
                      $"- سبب الإلغاء المتكرر: {perf.MostFrequentCancellationReason}"
                    : $"Performance of {deliveryMan.Name}:\n" +
                      $"- Assigned orders: {perf.AssignedCount}\n" +
                      $"- Delivered: {perf.DeliveredCount} ({perf.SuccessRate:F1}%)\n" +
                      $"- Cancelled: {perf.CancelledCount}\n" +
                      $"- Returned: {perf.ReturnedCount}\n" +
                      $"- Top cancellation reason: {perf.MostFrequentCancellationReason}";
            }

            // آخر طلب في مدينة
            if (lowerQuestion.Contains("last order in") ||
                lowerQuestion.Contains("who deliver last order") ||
                lowerQuestion.Contains("آخر طلب في") ||
                lowerQuestion.Contains("من قام بتوصيل آخر طلب"))
            {
                var cityName = ExtractCity(question);
                var city = await _unitOfWork.CityRepository.FindByNameAsync(cityName);

                if (city == null)
                {
                    return isArabic
                        ? $"لم يتم العثور على مدينة باسم '{cityName}'"
                        : $"City '{cityName}' not found";
                }

                var lastOrder = await _unitOfWork.OrderRepository.GetLastOrderInCityAsync(city.Id);

                if (lastOrder == null)
                {
                    return isArabic
                        ? $"لا يوجد طلبات في مدينة {city.Name}"
                        : $"No orders found in {city.Name}";
                }

                var deliveryMan = lastOrder.Assignments.FirstOrDefault()?.DeliveryMan;

                return isArabic
                    ? $"آخر طلب في {city.Name}:\n" +
                      $"- رقم الطلب: {lastOrder.Id}\n" +
                      $"- التاريخ: {lastOrder.CreationDate:yyyy-MM-dd}\n" +
                      $"- الموصل: {deliveryMan?.Name ?? "غير معين"}\n" +
                      $"- الحالة: {lastOrder.Status.Name}"
                    : $"Last order in {city.Name}:\n" +
                      $"- Order ID: {lastOrder.Id}\n" +
                      $"- Date: {lastOrder.CreationDate:yyyy-MM-dd}\n" +
                      $"- Delivery man: {deliveryMan?.Name ?? "Not assigned"}\n" +
                      $"- Status: {lastOrder.Status.Name}";
            }

            // تقرير الطلبات الملغاة
            if (lowerQuestion.Contains("cancelled order") ||
                lowerQuestion.Contains("report about cancelled") ||
                lowerQuestion.Contains("الطلبات الملغاة") ||
                lowerQuestion.Contains("تقرير إلغاء"))
            {
                var month = ExtractMonth(question);
                var startDate = month.HasValue
                    ? new DateTime(DateTime.Now.Year, month.Value, 1)
                    : DateTime.Now.AddMonths(-1);

                var endDate = month.HasValue
                    ? new DateTime(DateTime.Now.Year, month.Value, DateTime.DaysInMonth(DateTime.Now.Year, month.Value))
                    : DateTime.Now;

                var cancelledStatusId = await _statusRepository.GetStatusIdByNameAsync("Cancelled");
                var cancelledOrders = await _unitOfWork.OrderRepository.GetCancelledOrdersAsync(startDate, endDate);

                var reasons = cancelledOrders
                    .GroupBy(o => o.OrderCancellation.Reason)
                    .Select(g => new { Reason = g.Key, Count = g.Count() })
                    .OrderByDescending(g => g.Count)
                    .ToList();

                return isArabic
                    ? $"تقرير الطلبات الملغاة ({startDate:MMMM yyyy}):\n" +
                      $"- إجمالي الملغاة: {cancelledOrders.Count}\n" +
                      $"- أسباب الإلغاء:\n{string.Join("\n", reasons.Select(r => $"  • {r.Reason}: {r.Count}"))}"
                    : $"Cancelled orders report ({startDate:MMMM yyyy}):\n" +
                      $"- Total cancelled: {cancelledOrders.Count}\n" +
                      $"- Cancellation reasons:\n{string.Join("\n", reasons.Select(r => $"  • {r.Reason}: {r.Count}"))}";
            }

            // إحصائيات عامة
            if (lowerQuestion.Contains("statistics") ||
                lowerQuestion.Contains("report") ||
                lowerQuestion.Contains("إحصائيات") ||
                lowerQuestion.Contains("تقرير"))
            {
                var deliveredStatusId = await _statusRepository.GetStatusIdByNameAsync("Delivered");
                var cancelledStatusId = await _statusRepository.GetStatusIdByNameAsync("Cancelled");

                var orders = await _unitOfWork.OrderRepository.GetAllOrdersAsync();
                var deliveredCount = orders.Count(o => o.StatusId == deliveredStatusId);
                var cancelledCount = orders.Count(o => o.StatusId == cancelledStatusId);
                var successRate = orders.Count > 0 ? (double)deliveredCount / orders.Count * 100 : 0;

                return isArabic
                    ? "إحصائيات النظام:\n" +
                      $"- إجمالي الطلبات: {orders.Count}\n" +
                      $"- تم التوصيل: {deliveredCount} ({successRate:F1}%)\n" +
                      $"- تم الإلغاء: {cancelledCount}\n" +
                      $"- في انتظار التوصيل: {orders.Count - deliveredCount - cancelledCount}"
                    : "System statistics:\n" +
                      $"- Total orders: {orders.Count}\n" +
                      $"- Delivered: {deliveredCount} ({successRate:F1}%)\n" +
                      $"- Cancelled: {cancelledCount}\n" +
                      $"- Pending delivery: {orders.Count - deliveredCount - cancelledCount}";
            }

            // الرد الافتراضي
            return isArabic
                ? "عذرًا، لم أفهم سؤالك. يمكنك طرح أسئلة عن:\n- عدد الموصلين\n- أداء موصل معين\n- آخر طلب في مدينة\n- تقرير الطلبات الملغاة\n- إحصائيات التوصيل\n- حالة الطلبات اليوم\n- الطلبات المتأخرة\n- أسباب عدم التسليم\n- حالة الطلبات في مدينة محددة"
                : "I'm sorry, I didn't understand your question. You can ask about:\n- Number of delivery men\n- Performance of a specific delivery man\n- Last order in a city\n- Cancelled orders report\n- Delivery statistics\n- Today's orders status\n- Delayed orders\n- Reasons for non-delivery\n- Order status in specific city";
        }

        private bool ContainsArabic(string text)
        {
            foreach (char c in text)
            {
                if (c >= 0x0600 && c <= 0x06FF)
                    return true;
            }
            return false;
        }

        private string ExtractName(string question)
        {
            var keywords = new[] { "about", "for", "of", "عن", "لموصل", "للموصل" };
            foreach (var keyword in keywords)
            {
                int index = question.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                {
                    return question.Substring(index + keyword.Length).Trim();
                }
            }
            return question;
        }

        private string ExtractCity(string question)
        {
            var keywords = new[] { "in", "at", "inside", "بمدينة", "في", "لمدينة" };
            foreach (var keyword in keywords)
            {
                int index = question.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                {
                    return question.Substring(index + keyword.Length).Trim();
                }
            }
            return question;
        }

        private int? ExtractMonth(string question)
        {
            var months = new[] { "january", "february", "march", "april", "may", "june",
                               "july", "august", "september", "october", "november", "december",
                               "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو",
                               "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر" };

            for (int i = 0; i < months.Length; i++)
            {
                if (question.Contains(months[i], StringComparison.OrdinalIgnoreCase))
                {
                    return (i % 12) + 1;
                }
            }

            for (int i = 1; i <= 12; i++)
            {
                if (question.Contains(i.ToString()))
                {
                    return i;
                }
            }

            return null;
        }

        // ======== الطرق الجديدة ========
        private async Task<string> GetTodaysOrdersSummaryAsync(bool isArabic)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var orders = await _unitOfWork.OrderRepository.GetByDateRangeAsync(today, tomorrow);

            var deliveredStatusId = await _statusRepository.GetStatusIdByNameAsync("Delivered");
            var cancelledStatusId = await _statusRepository.GetStatusIdByNameAsync("Cancelled");

            var deliveredCount = orders.Count(o => o.StatusId == deliveredStatusId);
            var pendingCount = orders.Count(o => o.StatusId != deliveredStatusId &&
                                               o.StatusId != cancelledStatusId);

            return isArabic
                ? $"طلبات اليوم:\n- إجمالي الطلبات: {orders.Count}\n- تم التسليم: {deliveredCount}\n- قيد الانتظار: {pendingCount}"
                : $"Today's orders:\n- Total orders: {orders.Count}\n- Delivered: {deliveredCount}\n- Pending: {pendingCount}";
        }

        private async Task<string> GetPendingOrdersDetailsAsync(bool isArabic)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // الحصول على معرفات الحالات أولاً
            var deliveredStatusId = await _statusRepository.GetStatusIdByNameAsync("Delivered");
            var cancelledStatusId = await _statusRepository.GetStatusIdByNameAsync("Cancelled");

            // استدعاء البيانات مع التضمينات المطلوبة
            var orders = await _unitOfWork.OrderRepository.GetByDateRangeWithDetailsAsync(
                today,
                tomorrow,
                includeAssignments: true,
                includeDeliveryMan: true);

            var pendingOrders = orders
                .Where(o => o.StatusId != deliveredStatusId && o.StatusId != cancelledStatusId)
                .ToList();

            var groupedOrders = pendingOrders
                .GroupBy(o => o.Assignments.FirstOrDefault()?.DeliveryMan?.Name ?? "Not assigned")
                .Select(g => new {
                    DeliveryMan = g.Key,
                    Orders = g.ToList(),
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count);

            var response = new StringBuilder();

            if (isArabic)
            {
                response.AppendLine("الطلبات المتأخرة:");
                foreach (var group in groupedOrders)
                {
                    response.AppendLine($"- الموصل: {group.DeliveryMan} ({group.Count} طلب)");
                    response.AppendLine($"  الطلبات: {string.Join(", ", group.Orders.Select(o => o.Id))}");
                }
                response.AppendLine($"\nإجمالي الطلبات المتأخرة: {pendingOrders.Count}");
            }
            else
            {
                response.AppendLine("Delayed orders:");
                foreach (var group in groupedOrders)
                {
                    response.AppendLine($"- Delivery man: {group.DeliveryMan} ({group.Count} orders)");
                    response.AppendLine($"  Orders: {string.Join(", ", group.Orders.Select(o => o.Id))}");
                }
                response.AppendLine($"\nTotal delayed orders: {pendingOrders.Count}");
            }

            return response.ToString();
        }

        private async Task<string> GetNonDeliveryReasonsAsync(bool isArabic)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // استدعاء البيانات مع التضمينات المطلوبة
            var orders = await _unitOfWork.OrderRepository.GetByDateRangeWithDetailsAsync(
                today,
                tomorrow,
                includeCancellations: true);

            var cancelledStatusId = await _statusRepository.GetStatusIdByNameAsync("Cancelled");
            var cancelledOrders = orders
                .Where(o => o.StatusId == cancelledStatusId)
                .ToList();

            var reasons = cancelledOrders
                .GroupBy(o => o.OrderCancellation?.Reason ?? "Unknown")
                .Select(g => new { Reason = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count);

            var response = new StringBuilder();

            if (isArabic)
            {
                response.AppendLine("أسباب عدم التسليم اليوم:");
                foreach (var reason in reasons)
                {
                    response.AppendLine($"- {reason.Reason}: {reason.Count} طلب");
                }
            }
            else
            {
                response.AppendLine("Reasons for non-delivery today:");
                foreach (var reason in reasons)
                {
                    response.AppendLine($"- {reason.Reason}: {reason.Count} orders");
                }
            }

            return response.ToString();
        }

        private async Task<string> GetCityOrdersStatusAsync(string cityName, bool isArabic)
        {
            if (string.IsNullOrEmpty(cityName))
            {
                return isArabic
                    ? "يرجى تحديد المدينة (مثال: 'حالة الطلبات في مدينة نصر')"
                    : "Please specify city (e.g. 'order status in Nasr City')";
            }

            var city = await _unitOfWork.CityRepository.FindByNameAsync(cityName);
            if (city == null)
            {
                return isArabic
                    ? $"لم يتم العثور على مدينة باسم '{cityName}'"
                    : $"City '{cityName}' not found";
            }

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // استدعاء البيانات مع التضمينات المطلوبة
            var cityOrders = await _unitOfWork.OrderRepository.GetByCityAndDateRangeWithDetailsAsync(
                city.Id,
                today,
                tomorrow,
                includeAssignments: true,
                includeDeliveryMan: true);

            var deliveredStatusId = await _statusRepository.GetStatusIdByNameAsync("Delivered");
            var cancelledStatusId = await _statusRepository.GetStatusIdByNameAsync("Cancelled");

            var deliveredCount = cityOrders.Count(o => o.StatusId == deliveredStatusId);
            var delayedCount = cityOrders.Count(o => o.StatusId != deliveredStatusId &&
                                                  o.StatusId != cancelledStatusId);

            var delayedDetails = new StringBuilder();
            if (delayedCount > 0)
            {
                var delayedOrders = cityOrders
                    .Where(o => o.StatusId != deliveredStatusId && o.StatusId != cancelledStatusId)
                    .ToList();

                foreach (var order in delayedOrders)
                {
                    var deliveryMan = order.Assignments.FirstOrDefault()?.DeliveryMan?.Name ?? "Not assigned";
                    delayedDetails.AppendLine(isArabic
                        ? $"- الطلب #{order.Id}: {order.Status.Name} (الموصل: {deliveryMan})"
                        : $"- Order #{order.Id}: {order.Status.Name} (Delivery man: {deliveryMan})");
                }
            }

            return isArabic
                ? $"طلبات {city.Name} اليوم:\n" +
                  $"- إجمالي الطلبات: {cityOrders.Count}\n" +
                  $"- تم التسليم: {deliveredCount}\n" +
                  $"- متأخرة: {delayedCount}\n" +
                  (delayedCount > 0 ? $"تفاصيل التأخير:\n{delayedDetails}" : "")
                : $"Orders in {city.Name} today:\n" +
                  $"- Total orders: {cityOrders.Count}\n" +
                  $"- Delivered: {deliveredCount}\n" +
                  $"- Delayed: {delayedCount}\n" +
                  (delayedCount > 0 ? $"Delay details:\n{delayedDetails}" : "");
        }
    }
}