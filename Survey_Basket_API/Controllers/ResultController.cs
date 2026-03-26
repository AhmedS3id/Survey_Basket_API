using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Survey_Basket_API.Controllers
{
    [Route("api/polls/{pollId}/[controller]")]
    [ApiController]
    [Authorize]
    public class ResultController(IResultServices resultServices) : ControllerBase
    {
        private readonly IResultServices _resultServices = resultServices;

        [HttpGet("row-data")] 
        public async Task<IActionResult> PollVotesAsync([FromRoute] int pollId, CancellationToken cancellationToken)
        {
            var result = await _resultServices.GetPollVotesAsync(pollId, cancellationToken);
            return result.IsSuccess ? Ok(result.value) : result.ToProblem(); 
        }
        [HttpGet("votes-per-day")]
        public async Task<IActionResult> VotesPerDay([FromRoute] int pollId, CancellationToken cancellationToken)
        {
            var result = await _resultServices.GetVotesPerDayAsync(pollId, cancellationToken);
            return result.IsSuccess ? Ok(result.value) : result.ToProblem();
        }
    }
}
