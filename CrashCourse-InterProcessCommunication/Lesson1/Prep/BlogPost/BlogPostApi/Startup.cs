using BlogPostApi.Services;
using CrashCourseApi.Web.DataStores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReviewApi;
using Serilog;
using System;

namespace BlogPostApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _logger = new LoggerConfiguration()
                            .ReadFrom.Configuration(configuration)
                            .CreateLogger();

            _logger.Information("{ApplicationName} {Version} starting in ({Environment})", "CrashCourseApi", "1.0.0", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
        }

        public IConfiguration Configuration { get; }
        private readonly ILogger _logger;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {   // Map App Settings
                var settings = Configuration.GetSection("Settings").Get<Settings>();
                services.AddSingleton(settings);

                services.AddSingleton(_logger);
                services.AddControllers();
                services.AddTransient<IBlogPostService, BlogPostService>();
                services.AddSingleton<IBlogPostDataStore, BlogPostDataStore>();
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Failure when configuring the application.");
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            try
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseHttpsRedirection();

                app.UseRouting();

                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Failure when initializing the application.");
            }
        }
    }
}
