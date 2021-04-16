using CrashCourseApi.Review.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CrashCourseApi.Review.Web.Controllers
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
        public async Task<IActionResult> Get()
        {
            if (random.Next(1, 100) < _settings.InducedFailureRateFactor)
            {
                return UnprocessableEntity("Review GET endpoint failed");
            }

            _logger.Information("Review GET endpoint called");
            await Task.Delay(_settings.InducedLatencyFactor * 1000);
            return Ok("Review GET endpoint called");
        }

        // POST api/<ReviewController>
        [HttpPost]
        public IActionResult Post([FromBody] ReviewRequest request)
        {
            var flatRequest = JsonConvert.SerializeObject(request);

            _logger.Information("Review POST endpoint called");
            return Ok($"Review POST endpoint called with body request {flatRequest}");
        }
    }
}