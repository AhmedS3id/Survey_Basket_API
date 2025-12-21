
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

        [HttpGet]

        public IActionResult GetAll()
        {
            var polls = _pollServices.GetAll();
            var Response = polls.Adapt<IEnumerable<PollResponse>>();
            return Ok(Response);
        }

        [HttpGet("{id}")]

        public IActionResult Get([FromRoute] int id)
        {
            var poll = _pollServices.Get(id);

            if (poll == null)
                return NotFound();

            var response = poll.Adapt<PollResponse>();
            return Ok(response);

        }

        [HttpPost("")]
        public IActionResult Add([FromBody] PollRequest request)
        {
            var newpoll = _pollServices.Add(request.Adapt<Poll>());
            return CreatedAtAction(nameof(Get), new { id = newpoll.Id }, newpoll);

        }

        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] PollRequest request)
        {
            var is_updated = _pollServices.updated(id, request.Adapt<Poll>());
            if (!is_updated)
            {
                return NotFound();
            }
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var is_deleted = _pollServices.delete(id);
            if (!is_deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}
