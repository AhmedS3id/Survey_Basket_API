using Microsoft.AspNetCore.Authorization;
using Survey_Basket_API.Abstractions.Consts;
using Survey_Basket_API.Contract.Common;
using Survey_Basket_API.Contract.Questions;
using System.Threading.Tasks;

namespace Survey_Basket_API.Controllers
{
    [Route("api/polls/{pollId}/[controller]")]
    [ApiController]
   
    public class QuestionsController(IQuestionServices questionServices) : ControllerBase
    {
        private readonly IQuestionServices _questionServices = questionServices;
        [HttpGet("")]
        [HasPermission(Permissions.GetQuestions)]
        public async Task<IActionResult> GetAll([FromRoute] int pollId, [FromQuery] RequestFilter filter, CancellationToken cancellationToken)
        {
            var result = await _questionServices.GetAllAsync(pollId,filter,cancellationToken);
            return result.IsSuccess ? Ok(result.value) : result.ToProblem();     
        }


        [HttpGet("{id}")]
        [HasPermission(Permissions.GetQuestions)]
        public async Task< IActionResult> Get([FromRoute] int pollId, [FromRoute]int id ,CancellationToken cancellationToken)
        {
            var result = await _questionServices.GetAsync(pollId,id, cancellationToken);
            return result.IsSuccess ? Ok(result.value) : result.ToProblem();
        }

        [HttpPost("")]
        [HasPermission(Permissions.AddQuestions)]
        public async Task<IActionResult> Add([FromRoute] int pollId ,[FromBody] QuestionRequest request,CancellationToken cancellationToken)
        {
            var result = await _questionServices.AddAsync(pollId, request, cancellationToken);

            return result.IsSuccess ? CreatedAtAction(nameof(Get), new { pollId, result.value.Id }, result.value)
                : result.ToProblem();
        }
        [HttpPut ("{id}")]
        [HasPermission(Permissions.UpdateQuestions)]
        public async Task<IActionResult> Update([FromRoute] int pollId,int id, [FromBody] QuestionRequest request, CancellationToken cancellationToken)
        {
            var result = await _questionServices.UpdateAsync(pollId,id, request, cancellationToken);

            return result.IsSuccess?NoContent():result.ToProblem();
        }


        [HttpPut("{id}/toggleStatus")]
        [HasPermission(Permissions.UpdateQuestions)]
        public async Task<IActionResult> ToggleStatus([FromRoute] int pollId, [FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _questionServices.ToggleStatusAsync(pollId, id, cancellationToken);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
