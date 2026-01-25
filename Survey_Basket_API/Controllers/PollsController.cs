
using Microsoft.AspNetCore.Authorization;
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
            var poll = await _pollServices.GetAsync(id);

            if (poll == null)
                return NotFound();

            var response = poll.Adapt<PollResponse>();
            return Ok(response);

        }

        [HttpPost("")]
        public async Task<IActionResult> Add([FromBody] PollRequest request, CancellationToken cancellationToken = default)
        {
            var NewPoll = await _pollServices.AddAsync(request.Adapt<Poll>());

            return CreatedAtAction(nameof(Get), new { id = NewPoll.Id }, NewPoll.Adapt<PollResponse>());

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request,CancellationToken cancellationToken)
        {
            var is_updated = await _pollServices.updateAsync(id, request.Adapt<Poll>(),cancellationToken);
            if (!is_updated)
            {
                return NotFound();
            }
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id,CancellationToken cancellationToken)
        {
            var is_deleted = await _pollServices.deleteAsync(id,cancellationToken);
            if (!is_deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("{id}/togglePublish")]
        public async Task<IActionResult> TogglePublishStatus([FromRoute]int id,CancellationToken cancellationToken = default)
        {
            var is_updated = await _pollServices.TogglePublishStatusAsync(id,cancellationToken); 
            if (!is_updated)
            {
                return NotFound();
            }
            return NoContent();
        }



    }
}
