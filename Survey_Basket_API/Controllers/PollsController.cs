using Microsoft.AspNetCore.Authorization;
using Survey_Basket_API.Abstractions;
using Survey_Basket_API.Abstractions.Consts;
using Survey_Basket_API.Contract.Poll;
using System.Threading.Tasks;

namespace Survey_Basket_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class PollsController(IPollServices pollservices) : ControllerBase
    {
        private readonly IPollServices _pollServices = pollservices;

        //public PollsController(IPollServices pollServices)
        //{
        //    _pollServices = pollServices;
        //}
        [HttpGet("")]
        [HasPermission(Permissions.GetPolls)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var polls = await _pollServices.GetAllAsync(cancellationToken);
            return Ok(polls);
        }
        [HttpGet("current")]
        [Authorize(Roles = DefaultRoles.Member)]
        public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken = default)
        {
            var polls = await _pollServices.GetCurrentAsync(cancellationToken);
            return Ok(polls);
        }

        [HttpGet("{id}")]
        [HasPermission(Permissions.GetPolls)]
        public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            var result = await _pollServices.GetAsync(id);

            //if (result == null)
            //    return NotFound();

            //var response = result.Adapt<PollResponse>();
            //return Ok(response);
            return result.IsSuccess ? Ok(result.value) 
            :result.ToProblem();

        }

        [HttpPost("")]
        [HasPermission(Permissions.AddPolls)]
        public async Task<IActionResult> Add([FromBody] PollRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _pollServices.AddAsync(request,cancellationToken);

            return result.IsSuccess?
                CreatedAtAction(nameof(Get), new { id = result.value.Id },result.value)
                : result.ToProblem();

        }

        [HttpPut("{id}")]
        [HasPermission(Permissions.UpdatePolls)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
        {
            var result = await _pollServices.UpdateAsync(id, request, cancellationToken);

            return result.IsSuccess ? NoContent() : result.ToProblem();

        }
        [HttpDelete("{id}")]
        [HasPermission(Permissions.DeletePolls)]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _pollServices.DeleteAsync(id, cancellationToken);
           return result.IsSuccess?NoContent() : result.ToProblem();   
        }

        [HttpPut("{id}/togglePublish")]
        [HasPermission(Permissions.UpdatePolls)]
        public async Task<IActionResult> TogglePublishStatus([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            var result = await _pollServices.TogglePublishStatusAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                return result.ToProblem();
            }
            return NoContent();
        }



    }
}
