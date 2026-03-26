using Survey_Basket_API.Contract.Questions;
using Survey_Basket_API.Contract.Results;
using Survey_Basket_API.Entities;
using Survey_Basket_API.Errors;
using Survey_Basket_API.Persistence;

namespace Survey_Basket_API.Services
{
    public class ResultServices(AppDbContext context) : IResultServices
    {
        private readonly AppDbContext _context = context;

        public async Task<Result<PollVoteResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken)
        {
            var pollVotes =await _context.Polls
                .Where(x => x.Id == pollId)
                .Select(x => new PollVoteResponse(
                    x.Title,
                    x.Votes.Select(v => new VoteResponse(
                        $"{v.User.FirsName} {v.User.LastName}",
                        v.SubmittedOn,
                        v.VoteAnswer.Select(a => new QuestionAnswerResponse(
                            a.Question.Content,
                            a.Answer.Content
                            ))
                        ))

                    )).SingleOrDefaultAsync(cancellationToken);

            return pollVotes is null?
                Result.Failure<PollVoteResponse>(PollsErrors.InvalidPolls)
                :Result.success(pollVotes);
        }
        public async Task<Result<IEnumerable<VotePerDayResponse>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default)
        {
            var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);

            if (!pollIsExists)
                return Result.Failure<IEnumerable<VotePerDayResponse>>(PollsErrors.InvalidPolls);

            var votesPerDay = await _context.Votes
                .Where(x => x.PollId == pollId)
                .GroupBy(x => new { Date = DateOnly.FromDateTime(x.SubmittedOn) })
                .Select(g => new VotePerDayResponse(
                    g.Key.Date,
                    g.Count()
                ))
                .ToListAsync(cancellationToken);

            return Result.success<IEnumerable<VotePerDayResponse>>(votesPerDay);
        }
    }
}
