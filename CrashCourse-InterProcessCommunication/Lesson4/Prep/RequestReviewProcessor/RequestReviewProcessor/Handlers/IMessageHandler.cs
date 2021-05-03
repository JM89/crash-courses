using RequestReviewProcessor.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace RequestReviewProcessor.Handlers
{
    public interface IMessageHandler
    {
        Task<bool> ProcessMessageAsync(ReviewRequest request, CancellationToken cancellationToken);
    }
}
