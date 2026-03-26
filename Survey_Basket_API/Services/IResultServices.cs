using Survey_Basket_API.Contract.Results;

namespace Survey_Basket_API.Services
{
    public interface IResultServices
    {
        public Task<Result<PollVoteResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken);
        public Task<Result<IEnumerable<VotePerDayResponse>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken);
    }
}
