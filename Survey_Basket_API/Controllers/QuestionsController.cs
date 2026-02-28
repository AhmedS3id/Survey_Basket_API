using Microsoft.AspNetCore.Authorization;
using Survey_Basket_API.Contract.Questions;
using System.Threading.Tasks;

namespace Survey_Basket_API.Controllers
{
    [Route("api/polls/{pollId}/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionsController(IQuestionServices questionServices) : ControllerBase
    {
        private readonly IQuestionServices _questionServices = questionServices;
        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromRoute] int pollId,CancellationToken cancellationToken)
        {
            var result = await _questionServices.GetAllAsync(pollId, cancellationToken);
            return result.IsSuccess ? Ok(result.value) : result.ToProblem();     
        }


        [HttpGet("{id}")]
      public async Task< IActionResult> Get([FromRoute] int pollId, [FromRoute]int id ,CancellationToken cancellationToken)
        {
            var result = await _questionServices.GetAsync(pollId,id, cancellationToken);
            return result.IsSuccess ? Ok(result.value) : result.ToProblem();
        }

        [HttpPost("")]
       public async Task<IActionResult> Add([FromRoute] int pollId ,[FromBody] QuestionRequest request,CancellationToken cancellationToken)
        {
            var result = await _questionServices.AddAsync(pollId, request, cancellationToken);

            return result.IsSuccess ? CreatedAtAction(nameof(Get), new { pollId, result.value.Id }, result.value)
                : result.ToProblem();
        }
        [HttpPut ("{id}")]
        public async Task<IActionResult> Update([FromRoute] int pollId,int id, [FromBody] QuestionRequest request, CancellationToken cancellationToken)
        {
            var result = await _questionServices.UpdateAsync(pollId,id, request, cancellationToken);

            return result.IsSuccess?NoContent():result.ToProblem();
        }


        [HttpPut("{id}/toggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromRoute] int pollId, [FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _questionServices.ToggleStatusAsync(pollId, id, cancellationToken);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
