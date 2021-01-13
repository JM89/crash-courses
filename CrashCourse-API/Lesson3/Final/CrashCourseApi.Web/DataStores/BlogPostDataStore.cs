using CrashCourseApi.Web.Models;
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
            var blogPosts = new List<BlogPost>();

            var conn = new SqlConnection("Data Source=localhost,1433;Initial Catalog=CrashCourseDB;User ID=sa;Password=VerySecret1234!");
            conn.Open();

            var command = new SqlCommand("Select BlogPostId, Title, Content, CreationDate from [BlogPost]", conn);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var blogPost = new BlogPost()
                    {
                        Id = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        Content = reader.GetString(2),
                        CreationDate = reader.GetDateTime(3)
                    };
                    blogPosts.Add(blogPost);
                }
            }

            conn.Close();
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
