using BlogPostApi.Contracts;

namespace BlogPostApi.Services
{
    public interface IReviewRequestSender
    {
        bool SendMessage(ReviewRequest request);
    }
}
