using CrashCourseApi.Web.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CrashCourseApi.Web.DataStores
{
    public class BlogPostDataStore : IBlogPostDataStore
    {
        private readonly string _connectionString;

        public BlogPostDataStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CrashCourseDb");
        } 

        public void Delete(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Execute("DELETE FROM BlogPost WHERE BlogPostId = @Id", new { Id = id });

                conn.Close();
            }
        }

        public void Insert(BlogPost blogPost)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Execute("INSERT INTO BlogPost (Title, Content, CreationDate) VALUES (@Title, @Content, @CreationDate)", blogPost);

                conn.Close();
            }
        }

        public IEnumerable<BlogPost> SelectAll()
        {
            IEnumerable<BlogPost> blogPosts = null;
            using (var conn = new SqlConnection(_connectionString))
            {
                blogPosts = conn.Query<BlogPost>("Select BlogPostId as Id, Title, Content, CreationDate from [BlogPost]").AsList();

                conn.Close();
            }
            return blogPosts;
        }

        public BlogPost SelectById(int id)
        {
            BlogPost blogPost = null;
            using (var conn = new SqlConnection(_connectionString))
            {
                blogPost = conn.QuerySingleOrDefault<BlogPost>("Select BlogPostId as Id, Title, Content, CreationDate from [BlogPost] where BlogPostId = @Id", new { Id = id });

                conn.Close();
            }
            return blogPost;
        }

        public void Update(BlogPost blogPost)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Execute("UPDATE BlogPost SET Title = @Title, Content = @Content WHERE BlogPostId = @Id", blogPost);

                conn.Close();
            }
        }
    }
}
