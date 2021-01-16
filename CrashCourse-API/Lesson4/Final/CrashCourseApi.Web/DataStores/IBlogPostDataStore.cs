using CrashCourseApi.Web.Models;
using System;
using System.Collections.Generic;

namespace CrashCourseApi.Web.DataStores
{
    public interface IBlogPostDataStore
    {
        IEnumerable<BlogPost> SelectAll();
        Tuple<BlogPost, bool> SelectById(int id);
        bool Insert(BlogPost blogPost);
        bool Update(BlogPost blogPost);
        bool Delete(int id);
    }
}
