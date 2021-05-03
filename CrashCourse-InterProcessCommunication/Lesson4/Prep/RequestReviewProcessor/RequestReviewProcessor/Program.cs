using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RequestReviewProcessor.Handlers;
using Serilog;
using System;

namespace RequestReviewProcessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // Configure AppSettings
                    var settings = hostContext.Configuration.GetSection("Settings").Get<Settings>();
                    services.AddSingleton(settings);

                    // Configure SEQ Logging
                    var logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(hostContext.Configuration)
                        .CreateLogger();
                    services.AddSingleton<ILogger>(logger);

                    // Configure SQS Client
                    services.AddSingleton<IAmazonSQS>(new AmazonSQSClient(new AmazonSQSConfig() { ServiceURL = settings.EndpointUrl }));
                    
                    // Configure Message Handler
                    services
                       .AddHttpClient<IMessageHandler, MessageHandler>(client =>
                       {
                           client.BaseAddress = new Uri(settings.ReviewApiBaseUrl);
                       });

                    // Configure Background Service
                    services.AddHostedService<Worker>();
                });
    }
}
