using CrashCourseApi.Web.Models;
using System.Collections.Generic;

namespace CrashCourseApi.Web.DataStores
{
    public interface IBlogPostDataStore
    {
        IEnumerable<BlogPost> SelectAll();
        BlogPost SelectById(int id);
        void Insert(BlogPost blogPost);
        void Update(BlogPost blogPost);
        void Delete(int id);
    }
}
