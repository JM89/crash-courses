using Amazon.SQS;
using App.Metrics;
using App.Metrics.Formatters.Prometheus;
using App.Metrics.Scheduling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RequestReviewProcessor.Handlers;
using Serilog;
using System;
using System.Threading.Tasks;

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

                    // Configure App.Metrics
                    var metrics = new MetricsBuilder()
                        .Configuration.Configure(opt => {
                            opt.GlobalTags.Add("service", settings.ServiceName);
                        })
                        .Report.OverHttp(options =>
                        {
                            options.HttpSettings.RequestUri = new Uri(settings.PushGatewayApiBaseUrl);
                            options.MetricsOutputFormatter = new MetricsPrometheusTextOutputFormatter();
                        })
                        .Build();
                    services.AddSingleton<IMetrics>(metrics);

                    // Configure a scheduler to send the reports to the Push Gateway endpoint every 10s
                    var scheduler = new AppMetricsTaskScheduler(TimeSpan.FromSeconds(10),
                        async () => {
                            await Task.WhenAll(metrics.ReportRunner.RunAllAsync());
                        });
                    scheduler.Start();

                    // Configure Background Service
                    services.AddHostedService<Worker>();
                });
    }
}
