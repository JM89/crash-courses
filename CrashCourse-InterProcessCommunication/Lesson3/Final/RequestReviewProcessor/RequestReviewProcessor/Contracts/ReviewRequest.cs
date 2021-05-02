using System.Collections.Generic;

namespace RequestReviewProcessor.Contracts
{
    public class ReviewRequest
    {
        public int BlogPostId { get; set; }
        public List<string> Reviewers { get; set; }
    }
}
