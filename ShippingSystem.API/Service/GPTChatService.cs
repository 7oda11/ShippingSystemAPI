using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ShippingSystem.Core.Interfaces.Service;

namespace ShippingSystem.API.Services
{
    public class GPTChatService : IGPTChatService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public GPTChatService(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
        }

        public async Task<string> AskAsync(string prompt)
        {
            // Create API request
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.openai.com/v1/chat/completions"
            );

            request.Headers.Add("Authorization", $"Bearer {_apiKey}");

            // Construct request body
            var requestBody = new
            {
                model = "gpt-3.5-turbo", // or "gpt-4"
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "You are a delivery performance analyst assistant. " +
                                  "Always respond in professional English regardless of the query language. " +
                                  "Understand all languages but reply only in English. " +
                                  "Base your analysis strictly on the provided data."
                    },
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                }
            };

            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            // Send request and parse response
            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return $"Error: {response.StatusCode} - {responseContent}";
            }

            using var jsonDoc = JsonDocument.Parse(responseContent);
            var root = jsonDoc.RootElement;

            return root
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
        }
    }
}