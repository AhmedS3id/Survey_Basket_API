using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Survey_Basket_API.Contract.Rquest;
using Survey_Basket_API.Mapping;
using Survey_Basket_API.Models;
using Survey_Basket_API.Services;
using System.Diagnostics.Contracts;

namespace Survey_Basket_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollsController (IPollServices pollservices) : ControllerBase
    {
      private readonly IPollServices _pollServices =pollservices ;

        //public PollsController(IPollServices pollServices)
        //{
        //    _pollServices = pollServices;
        //}

        [HttpGet]

        public IActionResult GetAll() 
        {
            var polls = _pollServices.GetAll();
            return Ok(polls.MappToResponse());
        }

        [HttpGet("{id}")]

        public IActionResult Get([FromRoute ]int id) 
        {
            var poll = _pollServices.Get(id) ;
            
            return poll is null ? NotFound() : Ok(poll.MappToResponse());
        }

        [HttpPost("")]
        public IActionResult Add([FromBody ]PollRequest request)
        {
            var newpoll  = _pollServices.Add(request.MappToPollRequest()) ;
            return CreatedAtAction(nameof(Get),new {id= newpoll.Id },newpoll);
        }

        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id,[FromBody]PollRequest request) 
        {
          var is_updated= _pollServices.updated(id, request.MappToPollRequest());
            if (!is_updated)
            {
                return NotFound();
            }
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute]int id)
        {
            var is_deleted=_pollServices.delete(id) ;
            if (!is_deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
       
        

    }
}
