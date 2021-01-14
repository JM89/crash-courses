using CrashCourseApi.Web.Models;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CrashCourseApi.Web.DataStores
{
    public class BlogPostDataStore : IBlogPostDataStore
    {
        public void Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public void Insert(BlogPost blogPost)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<BlogPost> SelectAll()
        {
            IEnumerable<BlogPost> blogPosts = null;
            using (var conn = new SqlConnection("Data Source=localhost,1433;Initial Catalog=CrashCourseDB;User ID=sa;Password=VerySecret1234!"))
            {
                blogPosts = conn.Query<BlogPost>("Select BlogPostId as Id, Title, Content, CreationDate from [BlogPost]").AsList();

                conn.Close();
            }
            return blogPosts;
        }

        public BlogPost SelectById(int id)
        {
            throw new System.NotImplementedException();
        }

        public void Update(BlogPost blogPost)
        {
            throw new System.NotImplementedException();
        }
    }
}
