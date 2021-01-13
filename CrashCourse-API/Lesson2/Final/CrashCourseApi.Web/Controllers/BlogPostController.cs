using CrashCourseApi.Web.Models;
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
        private static readonly List<BlogPostResponse> blogPosts = new List<BlogPostResponse> {
                new BlogPostResponse()
                {
                    Id = 1,
                    Title = "Best practices for writing C# code",
                    Content = "Blah",
                    CreationDate = new System.DateTime(2021, 1, 11)
                },
                new BlogPostResponse()
                {
                    Id = 2,
                    Title = "How to design a distributed system properly",
                    Content = "Blah",
                    CreationDate = new System.DateTime(2021, 1, 12)
                }
            };

        // GET: api/<BlogPostController>
        [HttpGet]
        public IEnumerable<BlogPostResponse> Get()
        {
            return blogPosts;
        }

        // GET api/<BlogPostController>/5
        [HttpGet("{id}")]
        public BlogPostResponse Get(int id)
        {
            return blogPosts.SingleOrDefault(x => x.Id == id);
        }

        // POST api/<BlogPostController>
        [HttpPost]
        public void Post([FromBody] BlogPostRequest value)
        {
            var blogPost = new BlogPostResponse()
            {
                Id = blogPosts.Max(x => x.Id) + 1,
                Title = value.Title,
                Content = value.Content,
                CreationDate = DateTime.UtcNow
            };

            blogPosts.Add(blogPost);
        }

        // PUT api/<BlogPostController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] BlogPostRequest value)
        {
            var blogPost = blogPosts.SingleOrDefault(x => x.Id == id);
            if (blogPost != null)
            {
                blogPost.Title = value.Title;
                blogPost.Content = value.Content;
            }
        }

        // DELETE api/<BlogPostController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var blogPost = blogPosts.SingleOrDefault(x => x.Id == id);
            if (blogPost != null)
            {
                blogPosts.Remove(blogPost);
            }
        }
    }
}
