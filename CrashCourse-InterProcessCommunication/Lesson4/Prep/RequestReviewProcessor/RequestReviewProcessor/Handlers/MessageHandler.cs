using Newtonsoft.Json;
using RequestReviewProcessor.Contracts;
using Serilog;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RequestReviewProcessor.Handlers
{
    public class MessageHandler : IMessageHandler
    {
        private readonly ILogger _logger;
        private readonly Settings _settings;
        private readonly HttpClient _httpClient;

        public MessageHandler(ILogger logger, Settings settings, HttpClient httpClient)
        {
            _logger = logger;
            _settings = settings;
            _httpClient = httpClient;
        }

        public async Task<bool> ProcessMessageAsync(ReviewRequest request, CancellationToken cancellationToken)
        {
            _logger.Information("{ServiceName}: Blog Post {BlogPostId} Request Review received", _settings.ServiceName, request.BlogPostId);

            try
            {
                var uri = new Uri($"{_settings.ReviewApiBaseUrl}/api/review");

                var requestBody = new StringContent(
                    JsonConvert.SerializeObject(request),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(uri, requestBody);

                var content = await response.Content.ReadAsStringAsync();

                response.EnsureSuccessStatusCode();

                _logger.Information("{ServiceName}: Call succeeded. Content Request: {content}", _settings.ServiceName, content);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("{ServiceName}: Unknown error {Message}", _settings.ServiceName, ex.Message, ex.StackTrace);
                throw;
            }
        }
    }
}
