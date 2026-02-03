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

        public async Task<Result<PollResponse>> AddAsync(PollRequest request, CancellationToken cancellationToken)
        {
            var IsExistingTitle = await _context.Polls.AnyAsync(x=>x.Title==request.Title);
            if (IsExistingTitle)
                return Result.Failure<PollResponse>(PollsErrors.DuplicatedTitle);

            var poll = request.Adapt<Poll>();
            await _context.AddAsync(poll, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.success( poll.Adapt<PollResponse>()) ;

        }

        public async Task<Result> updateAsync(int id, PollRequest request, CancellationToken cancellationToken)
        {
            var CurrentPoll = await _context.Polls.FindAsync(id, cancellationToken);
            if (CurrentPoll is null)
                 return Result.Failure(PollsErrors.InvalidPolls);

            var IsExistingTitle = await _context.Polls.AnyAsync(x => x.Title == request.Title && x.Id!=id,cancellationToken:cancellationToken);
            if (IsExistingTitle)
                return Result.Failure<PollResponse>(PollsErrors.DuplicatedTitle);

                CurrentPoll.Title = request.Title;
                CurrentPoll.Summary = request.Summary;
                CurrentPoll.StartsAt = request.StartsAt;
                CurrentPoll.EndsAt = request.EndsAt;


                await _context.SaveChangesAsync(cancellationToken);
                return Result.success();

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
