
using Microsoft.EntityFrameworkCore;
using Survey_Basket_API.Contract.Questions;
using Survey_Basket_API.Contract.Votes;
using Survey_Basket_API.Entities;
using Survey_Basket_API.Persistence;

namespace Survey_Basket_API.Services
{
    public class VoteServices(AppDbContext context) : IVoteServices
    {
        private readonly AppDbContext _context = context;

        public async Task<Result> AddAsync(int pollId, string UserId,VoteRequest request, CancellationToken cancellationToken)
        {
            var hasVot = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == UserId, cancellationToken);
            if (hasVot)
                return Result.Failure(VotesErrors.DuplicatedVote);

            var pollExist = await _context.Polls.AnyAsync(x => x.Id == pollId && x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow));
            if (!pollExist)
                return Result.Failure(PollsErrors.InvalidPolls);

            var availableQuestion = await _context.Questions
                .Where(x=>x.PollId==pollId&&x.IsActive)
                .Select(x=>x.Id)
                .ToListAsync(cancellationToken);
            if (!request.Answer.Select(x=>x.QuestionId).SequenceEqual(availableQuestion))
                return Result.Failure(VotesErrors.InvalidQuestion);
            var vote = new Vote
            {
                PollId = pollId,
                UserId = UserId,
                VoteAnswer = request.Answer.Adapt<IEnumerable<VoteAnswer>>().ToList()
            };
            await _context.Votes.AddAsync(vote);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.success();
        }
    }
}
