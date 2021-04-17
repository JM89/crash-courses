using System.Collections.Generic;

namespace ReviewApi
{
    public class ReviewRequest
    {
        public int BlogPostId { get; set; }
        public List<string> Reviewers { get; set; }
    }
}
