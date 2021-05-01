using BlogPostApi.Models;
using System;
using System.Collections.Generic;

namespace BlogPostApi.Services
{
    public interface IBlogPostService
    {
        IEnumerable<BlogPost> GetAll();
        Tuple<BlogPost, bool> GetById(int id);
        bool Insert(BlogPost blogPost);
        bool Update(BlogPost blogPost);
        bool Delete(int id);
    }
}
