using Survey_Basket_API.Contract.Response;
using Survey_Basket_API.Contract.Rquest;
using Survey_Basket_API.Controllers;
using Survey_Basket_API.Models;

namespace Survey_Basket_API.Mapping
{
    public static class Mapping
    {
        public static PollResponse MappToResponse(this Poll poll)
        {
            return new()
            {
                Id = poll.Id,
                Title = poll.Title,
                Description = poll.Description
            };
        }
        public static IEnumerable<PollResponse> MappToResponse(this IEnumerable<Poll> polls)
        {
            return polls.Select( MappToResponse);
           
        }
        public static Poll MappToPollRequest(this PollRequest poll) {
            return new()
            {
                Title = poll.Title,
                Description = poll.Description
            };
        }
    }
}
