using CrashCourseApi.Web.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CrashCourseApi.Web.DataStores
{
    public class BlogPostDataStore : IBlogPostDataStore
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;
        private readonly string TemplateException = "Database exception when calling {DataStore} {Method}";
        private readonly string DataStore = nameof(BlogPostDataStore);

        public BlogPostDataStore(IConfiguration configuration, ILogger logger)
        {
            _connectionString = configuration.GetConnectionString("CrashCourseDb");
            _logger = logger;
        } 

        public bool Delete(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Execute("DELETE FROM BlogPost WHERE BlogPostId = @Id", new { Id = id });
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, TemplateException, DataStore, "Delete");
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public bool Insert(BlogPost blogPost)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Execute("INSERT INTO BlogPost (Title, Content, CreationDate) VALUES (@Title, @Content, @CreationDate)", blogPost);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, TemplateException, DataStore, "Insert");
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public IEnumerable<BlogPost> SelectAll()
        {
            IEnumerable<BlogPost> blogPosts = null;
            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    blogPosts = conn.Query<BlogPost>("Select BlogPostId as Id, Title, Content, CreationDate from [BlogPost]").AsList();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, TemplateException, DataStore, "SelectAll");
                }
                finally
                {
                    conn.Close();
                }
            }
            return blogPosts;
        }

        public Tuple<BlogPost, bool> SelectById(int id)
        {
            BlogPost blogPost = null;
            bool result = false;
            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    blogPost = conn.QuerySingleOrDefault<BlogPost>("Select BlogPostId as Id, Title, Content, CreationDate from [BlogPost] where BlogPostId = @Id", new { Id = id });
                    result = true;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, TemplateException, DataStore, "SelectById");
                    result = false;
                }
                finally
                {
                    conn.Close();
                }
            }
            return new Tuple<BlogPost, bool>(blogPost, result);
        }

        public bool Update(BlogPost blogPost)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Execute("UPDATE BlogPost SET Title = @Title, Content = @Content WHERE BlogPostId = @Id", blogPost);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, TemplateException, DataStore, "SelectById");
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}
