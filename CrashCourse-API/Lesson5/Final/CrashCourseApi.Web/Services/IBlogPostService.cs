using CrashCourseApi.Web.Models;
using System;
using System.Collections.Generic;

namespace CrashCourseApi.Web.Services
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
