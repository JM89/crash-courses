using CrashCourseApi.Web.Models;
using CrashCourseApi.Web.Services;
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

        public BlogPostController(IBlogPostService blogPostService)
        {
            _blogPostService = blogPostService;
        }

        // GET: api/<BlogPostController>
        [HttpGet]
        public IEnumerable<BlogPostResponse> Get()
        {
            var blogPostEntities = _blogPostService.GetAll();

            return blogPostEntities.Select(x => new BlogPostResponse() {
                Id = x.Id, 
                Title = x.Title,
                Content = x.Content,
                CreationDate = x.CreationDate
            });
        }

        // GET api/<BlogPostController>/5
        [HttpGet("{id}")]
        public BlogPostResponse Get(int id)
        {
            var blogPostEntity = _blogPostService.GetById(id);
            if (blogPostEntity == null)
            {
                return null;
            }

            //var referencePictures = ParseReferencePicture(blogPostEntity.Content);
            
            return new BlogPostResponse()
            {
                Id = blogPostEntity.Id,
                Title = blogPostEntity.Title,
                Content = blogPostEntity.Content,
                CreationDate = blogPostEntity.CreationDate,
                //PictureReferences = referencePictures
            };
        }

        // POST api/<BlogPostController>
        [HttpPost]
        public void Post([FromBody] BlogPostRequest value)
        {
            var blogPost = new BlogPost()
            {
                Title = value.Title,
                Content = value.Content,
                CreationDate = DateTime.UtcNow
            };

            _blogPostService.Insert(blogPost);
        }

        // PUT api/<BlogPostController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] BlogPostRequest value)
        {
            var blogPost = new BlogPost()
            {
                Id = id,
                Title = value.Title,
                Content = value.Content
            };

            _blogPostService.Update(blogPost);
        }

        // DELETE api/<BlogPostController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _blogPostService.Delete(id);
        }
    }
}