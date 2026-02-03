
using Microsoft.AspNetCore.Authorization;
using Survey_Basket_API.Abstractions;
using Survey_Basket_API.Contract.Poll;
using System.Threading.Tasks;

namespace Survey_Basket_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PollsController(IPollServices pollservices) : ControllerBase
    {
        private readonly IPollServices _pollServices = pollservices;

        //public PollsController(IPollServices pollServices)
        //{
        //    _pollServices = pollServices;
        //}
        [HttpGet("")]

        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var polls = await _pollServices.GetAllAsync();
            var Response = polls.Adapt<IEnumerable<PollResponse>>();
            return Ok(Response);
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            var result = await _pollServices.GetAsync(id);

            //if (result == null)
            //    return NotFound();

            //var response = result.Adapt<PollResponse>();
            //return Ok(response);
            return result.IsSuccess ? Ok(result.value) 
            :result.ToProblem(StatusCodes.Status400BadRequest);

        }

        [HttpPost("")]
        public async Task<IActionResult> Add([FromBody] PollRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _pollServices.AddAsync(request,cancellationToken);

            return result.IsSuccess?
                CreatedAtAction(nameof(Get), new { id = result.value.Id },result.value)
                : result.ToProblem(StatusCodes.Status409Conflict);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
        {
            var result = await _pollServices.updateAsync(id, request, cancellationToken);

            if (result.Error == PollsErrors.DuplicatedTitle)
                return result.ToProblem(StatusCodes.Status409Conflict);

            return result.IsSuccess ? NoContent() : result.ToProblem(StatusCodes.Status404NotFound);

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _pollServices.deleteAsync(id, cancellationToken);
           return result.IsSuccess?NoContent() : result.ToProblem(StatusCodes.Status400BadRequest);   
        }

        [HttpPut("{id}/togglePublish")]
        public async Task<IActionResult> TogglePublishStatus([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            var result = await _pollServices.TogglePublishStatusAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                return result.ToProblem(StatusCodes.Status400BadRequest);
            }
            return NoContent();
        }



    }
}
