using CrashCourseApi.Web.DataStores;
using CrashCourseApi.Web.Models;
using System.Collections.Generic;

namespace CrashCourseApi.Web.Services
{
    public class BlogPostService: IBlogPostService
    {
        private readonly IBlogPostDataStore _blogPostDataStore;

        public BlogPostService(IBlogPostDataStore blogPostDataStore)
        {
            _blogPostDataStore = blogPostDataStore;
        }

        public void Delete(int id)
        {
            _blogPostDataStore.Delete(id);
        }

        public IEnumerable<BlogPost> GetAll()
        {
            return _blogPostDataStore.SelectAll();
        }

        public BlogPost GetById(int id)
        {
            // Parse the content here
            return _blogPostDataStore.SelectById(id);
        }

        public void Insert(BlogPost blogPost)
        {
            _blogPostDataStore.Insert(blogPost);
        }

        public void Update(BlogPost blogPost)
        {
            _blogPostDataStore.Update(blogPost);
        }
    }
}
