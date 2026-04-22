
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Survey_Basket_API.Helpers;
using System.Security.Cryptography;
using System.Text;

namespace Survey_Basket_API.Services
{
    public class AuthServices(UserManager<ApplicationUser> UserManager,
        SignInManager <ApplicationUser>signInManager,
        ILogger<AuthServices>logger,
        IJwtProvider jwtProvider,
        IEmailSender emailSender,
        IHttpContextAccessor httpContextAccessor) : IAuthServices
    {
        private readonly UserManager<ApplicationUser> _UserManager = UserManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly ILogger<AuthServices> _logger = logger;
        private readonly IJwtProvider _jwtProvider = jwtProvider;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly int _RefreshTokenExpirationDate = 14;
        public async Task<Result<AuthResponse>> GetTokenAsync(string email, String Password, CancellationToken cancellationToken)
        {

            if (await _UserManager.FindByEmailAsync(email) is not { } user)
                return (Result.Failure<AuthResponse>(UserCredentials.InvalidCredentials));

            var result = await _signInManager.PasswordSignInAsync(user, Password, false,false);
            if (result.Succeeded)
            {
                var (token, expireIn) = _jwtProvider.GenerateToken(user);
                var RefreshToken = GenerateRefreshToken();
                var ExpirationDate = DateTime.UtcNow.AddDays(_RefreshTokenExpirationDate);

                user.RefreshTokens.Add(new RefreshTokens
                {
                    Token = RefreshToken,
                    ExpireOn = ExpirationDate
                });
                await _UserManager.UpdateAsync(user);
                var response = new AuthResponse(user.Id, user.Email, user.FirsName, user.LastName, token, expireIn, RefreshToken, ExpirationDate);
                return Result.success(response);
            }
            return Result.Failure<AuthResponse>(result.IsNotAllowed?UserCredentials.EmailNotConfirmed:UserCredentials.InvalidCredentials);
            
        }
        
        public async Task <Result>ConfirmationEmail(ConfirmEmailRequest request)
        {
            if (await _UserManager.FindByIdAsync(request.UserId) is not { } user)
            return Result.Failure(UserCredentials.InvalidCode);

            if (user.EmailConfirmed)
                return Result.Failure(UserCredentials.DuplicatedConfirmed);
            
            var code =request.Code;

            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            }catch (FormatException )
            {
                return Result.Failure(UserCredentials.InvalidCode);
            }

            var result = await _UserManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return Result.success();

            var error = result.Errors.First();
                return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

        }
        public async Task <Result> RegisterAsync(RegisterRequest request ,CancellationToken cancellationToken)
        {
            var emailIsExist = await _UserManager.Users.AnyAsync(x =>x.Email == request.Email, cancellationToken: cancellationToken);
            if (emailIsExist)
                return Result.Failure(UserCredentials.InvalidEmail);

            var user = request.Adapt<ApplicationUser>();
            user.FirsName=request.FirstName;

            var result = await _UserManager.CreateAsync(user,request.Password);

            if (result.Succeeded)
            {
                var code = await _UserManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                _logger.LogInformation("Confirmation code : {code}", code);

                await SendConfirmationEmail(user, code);

                return Result.success();
            }
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        public async Task<Result> ResendConfirmationEmail(ResendConfirmationEmailRequest request)
        {

            if (await _UserManager.FindByEmailAsync(request.Email) is not { } user)
                return Result.Failure(UserCredentials.InvalidCode);

            if (user.EmailConfirmed)
                return Result.Failure(UserCredentials.DuplicatedConfirmed);

            var code = await _UserManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            _logger.LogInformation("Confirmation code : {code}", code);

            await SendConfirmationEmail(user, code);

            return Result.success();

        }

        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string Token, string RefreshToken, CancellationToken cancellationToken)
        {
            var user_Id = _jwtProvider.ValidateToken(Token);
            if (user_Id is null)
                return Result.Failure<AuthResponse>(UserCredentials.InvalidJwtToken);

            var user = await _UserManager.FindByIdAsync(user_Id);
            if (user is null)
                return Result.Failure<AuthResponse>(UserCredentials.InvalidJwtToken);

            var UserRefreshToken = user.RefreshTokens
                .SingleOrDefault(x => x.Token == RefreshToken && x.Is_Active);
            if (UserRefreshToken is null)
                return Result.Failure<AuthResponse>(UserCredentials.InvalidRefreshToken);

            UserRefreshToken.RevokedOn = DateTime.UtcNow;

            var (Newtoken, expireIn) = _jwtProvider.GenerateToken(user);
            var newRefreshToken = GenerateRefreshToken();
            var ExpirationDate = DateTime.UtcNow.AddDays(_RefreshTokenExpirationDate);

            user.RefreshTokens.Add(new RefreshTokens
            {
                Token = newRefreshToken,
                ExpireOn = ExpirationDate
            });
            await _UserManager.UpdateAsync(user);
            var result= new AuthResponse(user.Id, user.Email, user.FirsName, user.LastName, Newtoken, expireIn, newRefreshToken, ExpirationDate);
            return Result.success(result);

        }

        public async Task<Result> InvokeTokenAsync(string Token, string RefreshToken, CancellationToken cancellationToken)
        {
            var user_Id = _jwtProvider.ValidateToken(Token);
            if (user_Id is null)
                return Result.Failure<AuthResponse>(UserCredentials.InvalidJwtToken);

            var user = await _UserManager.FindByIdAsync(user_Id);
            if (user is null)
                return Result.Failure<AuthResponse> (UserCredentials.InvalidCredentials);

            var UserRefreshToken = user.RefreshTokens
                .SingleOrDefault(x => x.Token == RefreshToken && x.Is_Active);
            if (UserRefreshToken is null)
                return Result.Failure<AuthResponse>(UserCredentials.InvalidRefreshToken);

            UserRefreshToken.RevokedOn = DateTime.UtcNow;
            await _UserManager.UpdateAsync(user);
            return Result.success();
        }

        private async Task SendConfirmationEmail(ApplicationUser user,string code)
        {
            var Origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

            var EmailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation", new Dictionary<string, string>
                {
                    {"{{UserName}}",user.FirsName },
                    {"{{AppName}}" ,"Survey Basket"},
                    {"{{ConfirmationLink}}",$"{Origin}/auth/emailConfirmation?userId={user.Id}&code={code}" }
                });
            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "✅Survey Basket : Email Confirmation", EmailBody));
            await Task.CompletedTask;
        }
    }
}
