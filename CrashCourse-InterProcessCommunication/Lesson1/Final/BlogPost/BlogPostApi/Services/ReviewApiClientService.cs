using BlogPostApi.Contracts;
using Newtonsoft.Json;
using ReviewApi;
using Serilog;
using System;
using System.Net.Http;
using System.Text;

namespace BlogPostApi.Services
{
    public class ReviewApiClientService : IReviewApiClientService
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly Settings _settings;

        public ReviewApiClientService(ILogger logger, HttpClient httpClient, Settings settings)
        {
            _logger = logger;
            _httpClient = httpClient;
            _settings = settings;
        }

        public bool Post(ReviewRequest request)
        {
            try
            {
                var uri = new Uri($"{_settings.ReviewApiBaseUrl}/api/review");

                var requestBody = new StringContent(
                    JsonConvert.SerializeObject(request), 
                    Encoding.UTF8, 
                    "application/json"
                );

                var response = _httpClient.PostAsync(uri, requestBody).GetAwaiter().GetResult();

                var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();

                _logger.Information($"Call succeeded. Content Request: {content}");
                return true;
            }
            catch(Exception ex)
            {
                _logger.Error($"Unknown error {ex.Message}", ex.StackTrace);
                throw;
            }
        }
    }
}
