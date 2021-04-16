using System.Collections.Generic;

namespace CrashCourseApi.Review.Web.Models
{
    public class ReviewRequest
    {
        public int BlogPostId { get; set; }
        public List<string> Reviewers { get; set; }
    }
}
