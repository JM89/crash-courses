using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Scheduling;
using BlogPostApi.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CrashCourseApi.Web.DataStores
{
    public class BlogPostDataStore : IBlogPostDataStore
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;
        private readonly string TemplateException = "Database exception when calling {DataStore} {Method}";
        private readonly string DataStore = nameof(BlogPostDataStore);
        private readonly IMetrics _metrics;

        private readonly static CounterOptions _sqlErrorCounterOptions = new CounterOptions() {
            MeasurementUnit = Unit.Errors,
            Name = "sql-errors"
        };

        public BlogPostDataStore(IConfiguration configuration, ILogger logger, IMetrics metrics)
        {
            _connectionString = configuration.GetConnectionString("CrashCourseDb");
            _logger = logger;
            _metrics = metrics;

            var scheduler = new AppMetricsTaskScheduler(
                TimeSpan.FromMinutes(1),
                () => {
                    metrics.Provider.Counter.Instance(_sqlErrorCounterOptions).Reset();
                    return Task.CompletedTask;
                });
           scheduler.Start();
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
                    _metrics.Measure.Counter.Increment(_sqlErrorCounterOptions);
                    _logger.Error(ex, TemplateException, DataStore, "Delete");
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public (int,bool) Insert(BlogPost blogPost)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    var id = conn.QuerySingle<int>(
                        "INSERT INTO BlogPost (Title, Content, CreationDate) VALUES (@Title, @Content, @CreationDate);SELECT @@IDENTITY;"
                        , blogPost);
                    return (id, true);
                }
                catch (Exception ex)
                {
                    _metrics.Measure.Counter.Increment(_sqlErrorCounterOptions);
                    _logger.Error(ex, TemplateException, DataStore, "Insert");
                    return (-1, false);
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
                    _metrics.Measure.Counter.Increment(_sqlErrorCounterOptions);
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
                    _metrics.Measure.Counter.Increment(_sqlErrorCounterOptions);
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
                    _metrics.Measure.Counter.Increment(_sqlErrorCounterOptions);
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
