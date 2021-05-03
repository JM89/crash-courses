using App.Metrics;
using App.Metrics.Timer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReviewApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly Settings _settings;
        private readonly Random random = new Random();
        private readonly IMetrics _metrics;

        private readonly static TimerOptions _timerOptions = new TimerOptions
        {
            Name = "Request Timer",
            MeasurementUnit = Unit.Requests,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds,
            Tags = new MetricTags(new string[] { "resource" }, new string[] { "review" })
        };

        private MetricTags AddEndpointName(string endpointName) => new MetricTags(new string[] { "endpoint" }, new string[] { endpointName });

        public ReviewController(ILogger logger, Settings settings, IMetrics metrics)
        {
            _logger = logger;
            _settings = settings;
            _metrics = metrics;
        }

        // GET: api/<ReviewController>
        [HttpGet]
        public IActionResult Get()
        {
            using (var time = _metrics.Measure.Timer.Time(_timerOptions, AddEndpointName("do-nothing")))
            {
                return Ok("Does nothing");
            }
        }

        // POST api/<ReviewController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReviewRequest request, CancellationToken token)
        {
            using (var time = _metrics.Measure.Timer.Time(_timerOptions, AddEndpointName("post")))
            {
                _logger.Information("Review POST endpoint called");

                if (random.Next(1, 100) < _settings.InducedFailureRateFactor)
                {
                    _logger.Error("Review GET endpoint failed");
                    return UnprocessableEntity("Review GET endpoint failed");
                }

                await Task.Delay(_settings.InducedLatencyFactor * 1000, token);

                var flatRequest = JsonConvert.SerializeObject(request);

                return Ok($"Review POST endpoint called with body request {flatRequest}");
            }
        }
    }
}