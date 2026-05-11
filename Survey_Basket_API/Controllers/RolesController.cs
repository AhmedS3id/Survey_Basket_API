using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Survey_Basket_API.Abstractions.Consts;
using Survey_Basket_API.Contract.Roles;
using System.Threading.Tasks;

namespace Survey_Basket_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController(IRoleServices roleServices) : ControllerBase
    {
        private readonly IRoleServices _roleServices = roleServices;

        [HttpGet("")]
        [HasPermission(Permissions.GetRoles)]
        public async Task<IActionResult> GetAll([FromQuery] bool IncludeDisable,CancellationToken cancellationToken)
        {
            var roles = await _roleServices.GetAllAsync(IncludeDisable,cancellationToken);
            return Ok(roles);
        } 
        [HttpGet("{id}")]
        [HasPermission(Permissions.GetRoles)]
        public async Task<IActionResult> GetById([FromRoute]string id,CancellationToken cancellationToken)
        {
            var result = await _roleServices.GetByIdAsync(id,cancellationToken);

            return result.IsFailure?result.ToProblem():Ok(result.value);
            //if (result.IsFailure)
            //    return NotFound();
            //return Ok(result.value);

        } 
        [HttpPost("")]
        [HasPermission(Permissions.AddRoles)]
        public async Task<IActionResult> Add([FromBody]RolesRequest request,CancellationToken cancellationToken)
        {
            var result = await _roleServices.AddAsync(request,cancellationToken);

            return result.IsFailure?result.ToProblem():Ok(result.value);
        } 
        [HttpPut("{id}")]
        [HasPermission(Permissions.UpdateRoles)]
        public async Task<IActionResult> Update([FromRoute]string id,[FromBody]RolesRequest request,CancellationToken cancellationToken)
        {
            var result = await _roleServices.UpdateAsync(id, request, cancellationToken);

            return result.IsFailure?result.ToProblem():NoContent();
        } 
        [HttpPut("{id}/toggle-status")]
        [HasPermission(Permissions.UpdateRoles)]
        public async Task<IActionResult> ToggleStatus([FromRoute]string id,CancellationToken cancellationToken)
        {
            var result = await _roleServices.ToggleStatus(id, cancellationToken);

            return result.IsFailure?result.ToProblem():NoContent();
        } 
    }
}
