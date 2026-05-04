using Survey_Basket_API.Abstractions.Consts;
using Survey_Basket_API.Contract.Votes;

namespace Survey_Basket_API.Controllers
{
    [Route("api/polls/{pollId}/vote")]
    [ApiController]
    //[Authorize]
    [Authorize (Roles = DefaultRoles.Member)]
    public class VotesController(IQuestionServices QuestionService,IVoteServices voteServices) : ControllerBase
    {
        private readonly IQuestionServices _questionService = QuestionService;
        private readonly IVoteServices _voteServices = voteServices;

        [HttpGet("")]
        
        public async Task<IActionResult> Start([FromRoute]int pollId,CancellationToken cancellationToken)
        {
            var user = "68050360-e7d4-40b6-8dcd-91b2f784dd40";//User.GetUserId();
            var result = await _questionService.GetCurrentAsync(pollId, user!, cancellationToken);
            return result.IsSuccess ? Ok(result.value) : result.ToProblem();

        }
        [HttpPost("")]
        public async Task<IActionResult> Vote([FromRoute] int pollId, VoteRequest request,CancellationToken cancellationToken)
        {
            var result =await _voteServices.AddAsync(pollId,User.GetUserId()!,request,cancellationToken);
          
            return result.IsSuccess? Created():result.ToProblem();

            
        }
    }
}
