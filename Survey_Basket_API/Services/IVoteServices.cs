using Survey_Basket_API.Contract.Votes;

namespace Survey_Basket_API.Services
{
    public interface IVoteServices
    {
        Task<Result> AddAsync(int PollId, string UserId,VoteRequest request, CancellationToken cancellationToken=default);

    }
}
