﻿using App.Metrics;
using App.Metrics.Timer;
using BlogPostApi.Models;
using BlogPostApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CrashCourseApi.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostController : ControllerBase
    {
        private readonly IBlogPostService _blogPostService;

        /// TODO: METRICS CODE
        private readonly IMetrics _metrics;

        private readonly static TimerOptions _timerOptions = new TimerOptions
        {
            Name = "Request Timer",
            MeasurementUnit = Unit.Requests,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds,
            Tags = new MetricTags(new string[] { "resource" }, new string[] { "blog-post" })
        };

        private MetricTags AddEndpointName(string endpointName) => new MetricTags(new string[] {"endpoint"}, new string[] {endpointName});

        public BlogPostController(IBlogPostService blogPostService, IMetrics metrics)
        {
            _blogPostService = blogPostService;
            _metrics = metrics;
        }

        // GET: api/<BlogPostController>
        [HttpGet]
        public IActionResult Get()
        {
            using (var time = _metrics.Measure.Timer.Time(_timerOptions, AddEndpointName("get-all")))
            { 
                var blogPostEntities = _blogPostService.GetAll();

                if (blogPostEntities == null)
                    return StatusCode(503);

                return Ok(blogPostEntities.Select(x => new BlogPostResponse()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Content = x.Content,
                    CreationDate = x.CreationDate
                }));
            }
        }

        // GET api/<BlogPostController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            using (var time = _metrics.Measure.Timer.Time(_timerOptions, AddEndpointName("get-by-id")))
            {
                var blogPostEntityResponse = _blogPostService.GetById(id);

                // If something went wrong:
                if (!blogPostEntityResponse.Item2)
                {
                    return StatusCode(503);
                }

                // If the item was not found
                // We now throw an error, but with a more descriptive message
                // Status code is now 404
                if (blogPostEntityResponse.Item1 == null)
                {
                    // To keep same format than other errors
                    var errors = new Dictionary<string, string[]>
                {
                    { "id", new string[] { $"Blog Post for ID '{id}' not found" } }
                };

                    return NotFound(new
                    {
                        status = 404,
                        errors = errors
                    });
                }

                var blogPostEntity = blogPostEntityResponse.Item1;
                return Ok(new BlogPostResponse()
                {
                    Id = blogPostEntity.Id,
                    Title = blogPostEntity.Title,
                    Content = blogPostEntity.Content,
                    CreationDate = blogPostEntity.CreationDate,
                    PictureReferences = blogPostEntity.PictureReferences
                });
            }
        }

        // POST api/<BlogPostController>
        [HttpPost]
        public IActionResult Post([FromBody] BlogPostRequest value)
        {
            using (var time = _metrics.Measure.Timer.Time(_timerOptions, AddEndpointName("create")))
            {
                var result = _blogPostService.Insert(new BlogPost()
                {
                    Title = value.Title,
                    Content = value.Content,
                    CreationDate = DateTime.UtcNow
                });
                return result ? Ok() : StatusCode(503);
            }
        }

        // PUT api/<BlogPostController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] BlogPostRequest value)
        {
            using (var time = _metrics.Measure.Timer.Time(_timerOptions, AddEndpointName("update")))
            {
                var result = _blogPostService.Update(new BlogPost()
                {
                    Id = id,
                    Title = value.Title,
                    Content = value.Content
                });
                return result ? Ok() : StatusCode(503);
            }
        }

        // DELETE api/<BlogPostController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using (var time = _metrics.Measure.Timer.Time(_timerOptions, AddEndpointName("delete")))
            {
                var result = _blogPostService.Delete(id);
                return result ? Ok() : StatusCode(503);
            }
        }
    }
}