using System.Collections.Generic;

namespace BlogPostApi.Contracts
{
    public class ReviewRequest
    {
        public int BlogPostId { get; set; }
        public List<string> Reviewers { get; set; }
    }
}
