
using Microsoft.AspNetCore.RateLimiting;

namespace Survey_Basket_API.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class AuthController(IAuthServices authServices, ILogger<AuthController> logger
        ) : ControllerBase
    {
        private readonly IAuthServices _authServices = authServices;
        private readonly ILogger _logger = logger;

        [HttpPost("")]
        public async Task<IActionResult> LoginAsync([FromBody]LoginRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Logging with Email : {email} and Password : {password}",request.Email,request.Password);
            var authResult = await _authServices.GetTokenAsync(request.Email, request.Password, cancellationToken);

            return authResult.IsSuccess ? Ok(authResult.value)
                : authResult.ToProblem();
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

            return authResult.IsSuccess ?  Ok(authResult.value):authResult.ToProblem();
        }

        [HttpPost("invoke-refresh-token")]
        public async Task<IActionResult> InvokeTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var InvokeResult = await _authServices.InvokeTokenAsync(request.Token, request.RefreshToken, cancellationToken);

            return InvokeResult.IsSuccess ? Ok() : InvokeResult.ToProblem();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var Result = await _authServices.RegisterAsync( request, cancellationToken);

            return Result.IsSuccess ? Ok() : Result.ToProblem();
        }

        [HttpPost("confirmed-email")]
        public async Task<IActionResult> EmailConfirmed([FromBody] ConfirmEmailRequest request)
        {
            var Result = await _authServices.ConfirmationEmail( request);

            return Result.IsSuccess ? Ok() : Result.ToProblem();
        }

        [HttpPost("resend-email-confirm")]
        public async Task<IActionResult> ResendEmailConfirmation([FromBody] ResendConfirmationEmailRequest request)
        {
            var Result = await _authServices.ResendConfirmationEmail( request);

            return Result.IsSuccess ? Ok() : Result.ToProblem();
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request)
        {
            var Result = await _authServices.ForgetPasswordAsync( request.Email);

            return Result.IsSuccess ? Ok() : Result.ToProblem();
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var Result = await _authServices.ResetPasswordAsync( request);

            return Result.IsSuccess ? Ok() : Result.ToProblem();
        }
        [HttpGet("test")]
        [EnableRateLimiting("concurrency")]
        public IActionResult Test()
        {
            Thread.Sleep(6000);
            return Ok();
        }

    }
}
