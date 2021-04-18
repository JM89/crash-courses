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

        public ReviewController(ILogger logger, Settings settings)
        {
            _logger = logger;
            _settings = settings;
        }

        // GET: api/<ReviewController>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Does nothing");
        }

        // POST api/<ReviewController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReviewRequest request, CancellationToken token)
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