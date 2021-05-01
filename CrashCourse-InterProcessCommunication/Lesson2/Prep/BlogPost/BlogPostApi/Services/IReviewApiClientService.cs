using BlogPostApi.Contracts;

namespace BlogPostApi.Services
{
    public interface IReviewApiClientService
    {
        bool Post(ReviewRequest request);
    }
}
