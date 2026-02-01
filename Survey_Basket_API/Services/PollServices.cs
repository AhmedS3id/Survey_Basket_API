using Azure.Core;
using Survey_Basket_API.Entities;
using Survey_Basket_API.Errors;
using Survey_Basket_API.Persistence;
using System.Threading.Tasks;

namespace Survey_Basket_API.Services
{
    public class PollServices(AppDbContext context) : IPollServices
    {
        private readonly AppDbContext _context = context;
       
        public async Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken ) =>
            await _context.Polls.AsNoTracking().ToListAsync( cancellationToken);



        public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken )
        {
            var poll=  await _context.Polls.FindAsync(id,cancellationToken);
            return poll is not null ? Result.success(poll.Adapt<PollResponse>()) : Result.Failure<PollResponse> (PollsErrors.InvalidPolls);
        }

        public async Task<PollResponse> AddAsync(PollRequest request, CancellationToken cancellationToken)
        {
            var poll = request.Adapt<Poll>();
            await _context.AddAsync(poll, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return poll.Adapt<PollResponse>() ;

        }

        public async Task<Result> updateAsync(int id, PollRequest poll, CancellationToken cancellationToken)
        {
            var CurrentPoll = await _context.Polls.FindAsync(id, cancellationToken);
            if (CurrentPoll != null)
            {
                CurrentPoll.Title = poll.Title;
                CurrentPoll.Summary = poll.Summary;
                CurrentPoll.StartsAt = poll.StartsAt;
                CurrentPoll.EndsAt = poll.EndsAt;


                await _context.SaveChangesAsync(cancellationToken);
                return Result.success();
            }
            else
            {
                return Result.Failure(PollsErrors.InvalidPolls);
            }
        }



        public async Task<Result> deleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var poll = await _context.Polls.FindAsync(id, cancellationToken);
            if (poll != null)
            {
                _context.Remove(poll);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.success();
            }

            return Result.Failure(PollsErrors.InvalidPolls);
        }

        public async Task<Result> TogglePublishStatusAsync(int id, CancellationToken cancellationToken)
        {
            var CurrentPoll = await _context.Polls.FindAsync(id, cancellationToken);
            if (CurrentPoll != null)
            {
                CurrentPoll.IsPublished = !CurrentPoll.IsPublished;

                await _context.SaveChangesAsync(cancellationToken);
                return Result.success();
            }
            else
            {
                return Result.Failure(PollsErrors.InvalidPolls);
            }
        }
    }
}
