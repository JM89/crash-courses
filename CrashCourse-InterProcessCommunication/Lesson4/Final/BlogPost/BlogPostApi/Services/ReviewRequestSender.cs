using Amazon.SQS;
using Amazon.SQS.Model;
using BlogPostApi.Contracts;
using Newtonsoft.Json;
using ReviewApi;
using Serilog;
using System;
using System.Collections.Generic;

namespace BlogPostApi.Services
{
    public class ReviewRequestSender : IReviewRequestSender
    {
        private readonly ILogger _logger;
        private readonly Settings _settings;
        private readonly IAmazonSQS _sqsClient;

        public ReviewRequestSender(ILogger logger, Settings settings, IAmazonSQS sqsClient)
        {
            _logger = logger;
            _settings = settings;
            _sqsClient = sqsClient;
        }

        public bool SendMessage(ReviewRequest request)
        {
            try
            {
                var queueUrl = $"{_settings.EndpointUrl}/{_settings.Account}/{_settings.QueueName}";

                var jsonRequest = JsonConvert.SerializeObject(request);

                var sqsRequest = new SendMessageRequest() { 
                    QueueUrl = queueUrl,
                    MessageBody = jsonRequest,
                    DelaySeconds = 1,
                    MessageAttributes = new Dictionary<string, MessageAttributeValue> {
                        { "Metadata", new MessageAttributeValue() { DataType = "String", StringValue = "48485a3953bb6124" } }
                    }
                };

                var response = _sqsClient.SendMessageAsync(sqsRequest).GetAwaiter().GetResult();

                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"Invalid status code: {response.HttpStatusCode}");
                }

                _logger.Information($"Call succeeded. Message ID: {response.MessageId}");

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
