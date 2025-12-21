

namespace Survey_Basket_API.Services
{
    public class PollServices : IPollServices
    {
        private readonly static List<Poll> _Polls = new List<Poll>
        {
        new Poll{Id=1,Title="My Title",Description="THIS IS MY FIRST API" }
        };
        public IEnumerable<Poll> GetAll()
        {
            return _Polls;
        }
        public Poll? Get(int id)
        {
           return _Polls.SingleOrDefault(x => x.Id == id);
        }

        public Poll Add(Poll poll)
        {
            poll.Id = _Polls.Count + 1;
            _Polls.Add(poll);
            return poll;
        
        }

        public bool updated(int id , Poll poll)
        {
           var CurrentPoll = Get(id);
            if (CurrentPoll != null)
            {
                CurrentPoll.Title = poll.Title ;
                 CurrentPoll.Description = poll.Description;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool delete(int id)
        {
           var poll = Get(id);
            if (poll != null)
            {
              _Polls.Remove(poll);
                return true;
            }
          
            return false;
        }
    }
}
