using Amazon.SQS;
using Amazon.SQS.Model;
using App.Metrics;
using App.Metrics.Timer;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RequestReviewProcessor.Contracts;
using RequestReviewProcessor.Handlers;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RequestReviewProcessor
{
    public class Worker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IAmazonSQS _sqsClient;
        private readonly IMessageHandler _messageHandler;
        private readonly Settings _settings;
        private readonly IMetrics _metrics;

        private readonly static TimerOptions _timerOptions = new TimerOptions
        {
            Name = "sqs messages handling",
            MeasurementUnit = Unit.Requests,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds
        };

        public Worker(ILogger logger, Settings settings, IAmazonSQS sqsClient, IMessageHandler messageHandler, IMetrics metrics)
        {
            _logger = logger;
            _settings = settings;
            _sqsClient = sqsClient;
            _messageHandler = messageHandler;
            _metrics = metrics;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var queueUrl = $"{_settings.EndpointUrl}/{_settings.Account}/{_settings.QueueName}";

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Create Receive Message Request 
                    var receiveMessageRequest = new ReceiveMessageRequest()
                    {
                        QueueUrl = queueUrl
                    };

                    // Call the Receive-Message AWS Commands
                    var receiveMessageResponse = await _sqsClient.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);

                    // For each message received in the batch 
                    foreach (var message in receiveMessageResponse.Messages)
                    {
                        using (var time = _metrics.Measure.Timer.Time(_timerOptions, new MetricTags("action", "process-message")))
                        {
                            // Log the message ID
                            _logger.Information("{ServiceName}: Message {MessageId} received", _settings.ServiceName, message.MessageId);

                            // Deserialize the content of the message
                            var requestReview = JsonConvert.DeserializeObject<ReviewRequest>(message.Body);
                            // Pass it through the Process Message method
                            await _messageHandler.ProcessMessageAsync(requestReview, stoppingToken);

                            // After processing the message, delete it from the queue (otherwise it will be reprocessed)
                            await _sqsClient.DeleteMessageAsync(new DeleteMessageRequest()
                            {
                                QueueUrl = queueUrl,
                                ReceiptHandle = message.ReceiptHandle
                            }, stoppingToken);
                        }
                    }

                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, ex.Message);
                    await Task.Delay(10000, stoppingToken);
                }
            }
        }
    }
}
