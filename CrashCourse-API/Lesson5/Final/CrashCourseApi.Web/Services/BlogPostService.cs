using CrashCourseApi.Web.DataStores;
using CrashCourseApi.Web.Models;
using Serilog;
using System;
using System.Collections.Generic;

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
            // TODO: Parse the content here
            return _blogPostDataStore.SelectById(id);
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
