using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CrashCourseApi.BlogPost.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostController : ControllerBase
    {
        private readonly ILogger _logger;
        public BlogPostController(ILogger logger)
        {
            _logger = logger;
        }

        // GET: api/<BlogPostController>
        [HttpGet]
        public IActionResult Get()
        {
            _logger.Information("BlogPost GET endpoint called");
            return Ok("BlogPost GET endpoint called");
        }
    }
}