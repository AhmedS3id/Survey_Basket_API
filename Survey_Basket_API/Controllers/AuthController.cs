
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using Survey_Basket_API.Authentication;
using Survey_Basket_API.Errors;

namespace Survey_Basket_API.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class AuthController(IAuthServices authServices,
        IOptions<JwtOptions> JwtOptions) : ControllerBase
    {
        private readonly IAuthServices _authServices = authServices;

       private readonly JwtOptions _Options = JwtOptions.Value;

        [HttpPost("")]
        public async Task<IActionResult> LoginAsync([FromBody]LoginRequest request, CancellationToken cancellationToken)
        {
            var authResult = await _authServices.GetTokenAsync(request.Email, request.Password, cancellationToken);

            return authResult.IsSuccess ? Ok(authResult.value)
                : authResult.ToProblem(StatusCodes.Status400BadRequest);
           //install package one off first
            //return authResult.Match(
            //    Ok,
            //    error => Problem(statusCode: StatusCodes.Status400BadRequest, title: error.Code, detail: error.Description)
            //);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var authResult = await _authServices.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

            return authResult.IsSuccess ?  Ok(authResult.value):authResult.ToProblem(StatusCodes.Status400BadRequest);
        }

        [HttpPost("invoke-refresh-token")]
        public async Task<IActionResult> InvokeTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var InvokeResult = await _authServices.InvokeTokenAsync(request.Token, request.RefreshToken, cancellationToken);

            return InvokeResult.IsSuccess ? Ok() : InvokeResult.ToProblem(StatusCodes.Status400BadRequest);
        }

    }
}
