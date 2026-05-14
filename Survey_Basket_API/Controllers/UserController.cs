using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Survey_Basket_API.Abstractions.Consts;
using System.Threading.Tasks;

namespace Survey_Basket_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet("")]
        [HasPermission(Permissions.GetUsers)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _userService.GetAllAsync());
        }
        [HttpGet("{id}")]
        [HasPermission(Permissions.GetUsers)]
        public async Task<IActionResult> GetAll([FromRoute] string id)
        {
            var result = await _userService.GetAsync(id);
            return result.IsSuccess ? Ok(result.value) : result.ToProblem();
        }
        [HttpPost("")]
        [HasPermission(Permissions.AddUsers)]
        public async Task<IActionResult> Add([FromBody] CreateUserRequest request)
        {
            var result = await _userService.AddAsync(request);
            return result.IsSuccess ? CreatedAtAction(nameof(GetAll),new {result.value.Id},result.value) : result.ToProblem();
        }
    }
}
