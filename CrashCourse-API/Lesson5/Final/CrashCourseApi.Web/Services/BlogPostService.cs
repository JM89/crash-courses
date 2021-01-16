using CrashCourseApi.Web.DataStores;
using CrashCourseApi.Web.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CrashCourseApi.Web.Services
{
    public class BlogPostService : IBlogPostService
    {
        private readonly IBlogPostDataStore _blogPostDataStore;
        private readonly ILogger _logger;

        public BlogPostService(IBlogPostDataStore blogPostDataStore, ILogger logger)
        {
            _blogPostDataStore = blogPostDataStore;
            _logger = logger;
        }

        public bool Delete(int id)
        {
            return _blogPostDataStore.Delete(id);
        }

        public IEnumerable<BlogPost> GetAll()
        {
            return _blogPostDataStore.SelectAll();
        }

        public Tuple<BlogPost, bool> GetById(int id)
        {
            var blogPost = _blogPostDataStore.SelectById(id);

            if (blogPost == null)
                return blogPost;

            var content = blogPost.Item1.Content;

            var regex = new Regex(@"(IMG:)\w+.png|(IMG:)\w+.jpg");
            var matches = regex.Matches(content);
            blogPost.Item1.PictureReferences = matches.Select(x => x.Value.Replace("IMG:", ""));

            return blogPost;
        }

        public bool Insert(BlogPost blogPost)
        {
            return _blogPostDataStore.Insert(blogPost);
        }

        public bool Update(BlogPost blogPost)
        {
            return _blogPostDataStore.Update(blogPost);
        }
    }
}
