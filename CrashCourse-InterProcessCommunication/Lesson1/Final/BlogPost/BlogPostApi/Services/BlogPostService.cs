using BlogPostApi.Contracts;
using BlogPostApi.Models;
using CrashCourseApi.Web.DataStores;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BlogPostApi.Services
{
    public class BlogPostService : IBlogPostService
    {
        private readonly IBlogPostDataStore _blogPostDataStore;
        private readonly ILogger _logger;
        private readonly IReviewApiClientService _reviewApiClientService;

        public BlogPostService(IBlogPostDataStore blogPostDataStore, ILogger logger, IReviewApiClientService reviewApiClientService)
        {
            _blogPostDataStore = blogPostDataStore;
            _logger = logger;
            _reviewApiClientService = reviewApiClientService;
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
            var (blogPostId, inserted) = _blogPostDataStore.Insert(blogPost);

            if (inserted)
            {
                var reviewRequest = new ReviewRequest()
                {
                    BlogPostId = blogPostId,
                    Reviewers = new List<string>() // Empty list for now
                };

                var result = _reviewApiClientService.Post(reviewRequest);
                _logger.Information($"Result: {result}");
            }

            return inserted;
        }

        public bool Update(BlogPost blogPost)
        {
            return _blogPostDataStore.Update(blogPost);
        }
    }
}
