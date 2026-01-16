

using Survey_Basket_API.Entities;
using Survey_Basket_API.Persistence;
using System.Threading.Tasks;

namespace Survey_Basket_API.Services
{
    public class PollServices(AppDbContext context) : IPollServices
    {
        private readonly AppDbContext _context = context;
       
        public async Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken ) =>
            await _context.Polls.AsNoTracking().ToListAsync( cancellationToken);



        public async Task<Poll?> GetAsync(int id, CancellationToken cancellationToken )
        {
           return  await _context.Polls.FindAsync(id,cancellationToken);
        }

        public  async Task< Poll> AddAsync(Poll poll, CancellationToken cancellationToken )
        {
            await _context.AddAsync(poll,cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return poll;

        }

        public async Task< bool> updateAsync(int id, Poll poll,CancellationToken cancellationToken)
        {
            var CurrentPoll =await GetAsync(id,cancellationToken);
            if (CurrentPoll != null)
            {
                CurrentPoll.Title = poll.Title;
                CurrentPoll.Summary = poll.Summary;
                CurrentPoll.StartsAt = poll.StartsAt;
                CurrentPoll.EndsAt = poll.EndsAt;
               

                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task < bool> deleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var poll =await GetAsync(id,cancellationToken);
            if (poll != null)
            {
                _context.Remove(poll);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }

            return false;
        }

        public async Task<bool> TogglePublishStatusAsync(int id, CancellationToken cancellationToken)
        {
            var CurrentPoll = await GetAsync(id, cancellationToken);
            if (CurrentPoll != null)
            {
                CurrentPoll.IsPublished = !CurrentPoll.IsPublished;

                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
