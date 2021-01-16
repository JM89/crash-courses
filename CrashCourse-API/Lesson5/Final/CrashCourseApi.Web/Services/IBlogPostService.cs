using CrashCourseApi.Web.Models;
using System.Collections.Generic;

namespace CrashCourseApi.Web.Services
{
    public interface IBlogPostService
    {
        IEnumerable<BlogPost> GetAll();
        BlogPost GetById(int id);
        void Insert(BlogPost blogPost);
        void Update(BlogPost blogPost);
        void Delete(int id);
    }
}
