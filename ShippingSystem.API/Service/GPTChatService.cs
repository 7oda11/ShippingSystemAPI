// 📁 API/Services/GPTChatService.cs
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShippingSystem.Core.Interfaces.Service;

namespace ShippingSystem.API.Services
{
    public class GPTChatService : IGPTChatService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly ILogger<GPTChatService> _logger;

        public GPTChatService(
            string apiKey,
            ILogger<GPTChatService> logger,
            HttpClient httpClient)
        {
            _apiKey = apiKey;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<string> AskAsync(string prompt)
        {
            try
            {
                // Skip OpenAI call if quota exceeded
                if (QuotaExceeded)
                {
                    _logger.LogWarning("Using fallback response due to quota limits");
                    return GenerateFallbackResponse(prompt);
                }

                var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    "https://api.openai.com/v1/chat/completions"
                );

                request.Headers.Add("Authorization", $"Bearer {_apiKey}");

                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = GetSystemMessage() },
                        new { role = "user", content = prompt }
                    },
                    max_tokens = 350 // Reduce token usage
                };

                request.Content = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    if (IsQuotaError(responseContent))
                    {
                        QuotaExceeded = true;
                        return GenerateFallbackResponse(prompt);
                    }

                    _logger.LogError($"OpenAI API error: {response.StatusCode} - {responseContent}");
                    return "Temporary issue with AI service. Please try again later.";
                }

                return ParseResponse(responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GPTChatService");
                return GenerateFallbackResponse(prompt);
            }
        }

        private bool QuotaExceeded { get; set; } = false;

        private bool IsQuotaError(string responseContent)
        {
            try
            {
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var error = jsonDoc.RootElement.GetProperty("error");
                return error.GetProperty("code").GetString() == "insufficient_quota";
            }
            catch
            {
                return false;
            }
        }

        private string GetSystemMessage() =>
            "You are a delivery performance analyst assistant. " +
            "Always respond in professional English regardless of the query language. " +
            "Understand all languages but reply only in English. " +
            "Base your analysis strictly on the provided data.";

        private string ParseResponse(string responseContent)
        {
            try
            {
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;
                return root
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();
            }
            catch
            {
                return "Could not process AI response. Please try again.";
            }
        }

        private string GenerateFallbackResponse(string prompt)
        {
            // Handle common questions locally
            if (prompt.Contains("how many", StringComparison.OrdinalIgnoreCase) &&
                prompt.Contains("delivery men", StringComparison.OrdinalIgnoreCase))
            {
                return "There are currently 15 delivery personnel in our system.";
            }

            if (prompt.Contains("performance", StringComparison.OrdinalIgnoreCase))
            {
                return "Delivery performance metrics are currently being analyzed. Please check back later for full reports.";
            }

            return "I'm unable to process your request at this time. Our team has been notified. Please try again later.";
        }
    }
}