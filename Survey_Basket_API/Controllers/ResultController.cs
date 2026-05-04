using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Survey_Basket_API.Abstractions.Consts;

namespace Survey_Basket_API.Controllers
{
    [Route("api/polls/{pollId}/[controller]")]
    [ApiController]
    [HasPermission(Permissions.Results)]
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

        [HttpGet("votes-per-question")]
        public async Task<IActionResult> VotesPerQuestion([FromRoute] int pollId, CancellationToken cancellationToken)
        {
            var result = await _resultServices.GetVotesPerQuestionAsync(pollId, cancellationToken);

            return result.IsSuccess ? Ok(result.value) : result.ToProblem();
        }
    }
}
