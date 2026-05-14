
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Survey_Basket_API.Abstractions.Consts;
using Survey_Basket_API.Helpers;
using Survey_Basket_API.Persistence;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Survey_Basket_API.Services
{
    public class AuthServices(UserManager<ApplicationUser> UserManager,
        SignInManager <ApplicationUser>signInManager,
        ILogger<AuthServices>logger,
        IJwtProvider jwtProvider,
        IEmailSender emailSender,
        AppDbContext context,
        IHttpContextAccessor httpContextAccessor) : IAuthServices
    {
        private readonly UserManager<ApplicationUser> _UserManager = UserManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly ILogger<AuthServices> _logger = logger;
        private readonly IJwtProvider _jwtProvider = jwtProvider;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly AppDbContext _context = context;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly int _RefreshTokenExpirationDate = 14;
        public async Task<Result<AuthResponse>> GetTokenAsync(string email, String Password, CancellationToken cancellationToken)
        {

            if (await _UserManager.FindByEmailAsync(email) is not { } user)
                return (Result.Failure<AuthResponse>(UserCredentials.InvalidCredentials));

            if (user.IsDisabled)
                return (Result.Failure<AuthResponse>(UserCredentials.DisableUser));

            var result = await _signInManager.PasswordSignInAsync(user, Password, false,true);
            if (result.Succeeded)
            {
                var (userRoles, Permission) = await GetRolesAndPermission(user, cancellationToken);

                var (token, expireIn) = _jwtProvider.GenerateToken(user, userRoles, Permission);
                var RefreshToken = GenerateRefreshToken();
                var ExpirationDate = DateTime.UtcNow.AddDays(_RefreshTokenExpirationDate);

                user.RefreshTokens.Add(new RefreshTokens
                {
                    Token = RefreshToken,
                    ExpireOn = ExpirationDate
                });
                await _UserManager.UpdateAsync(user);
                var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expireIn, RefreshToken, ExpirationDate);
                return Result.success(response);
            }

            var error = result.IsNotAllowed ?
                UserCredentials.EmailNotConfirmed :
                result.IsLockedOut ?
                UserCredentials.UserLockedOut :
                UserCredentials.InvalidCredentials;

            return Result.Failure<AuthResponse>(error);
            
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
            {
                await _UserManager.AddToRoleAsync(user, DefaultRoles.Member);
                return Result.success();
            }
                

            var error = result.Errors.First();
                return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

        }
        public async Task <Result> RegisterAsync(RegisterRequest request ,CancellationToken cancellationToken)
        {
            var emailIsExist = await _UserManager.Users.AnyAsync(x =>x.Email == request.Email, cancellationToken: cancellationToken);
            if (emailIsExist)
                return Result.Failure(UserCredentials.InvalidEmail);

            var user = request.Adapt<ApplicationUser>();
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

            if (user.IsDisabled)
                return (Result.Failure<AuthResponse>(UserCredentials.DisableUser));

            if (user.LockoutEnabled)
                return (Result.Failure<AuthResponse>(UserCredentials.UserLockedOut));

            var UserRefreshToken = user.RefreshTokens
                .SingleOrDefault(x => x.Token == RefreshToken && x.Is_Active);
            if (UserRefreshToken is null)
                return Result.Failure<AuthResponse>(UserCredentials.InvalidRefreshToken);

            UserRefreshToken.RevokedOn = DateTime.UtcNow;

            var (userRoles, Permission) = await GetRolesAndPermission(user, cancellationToken);

            var (Newtoken, expireIn) = _jwtProvider.GenerateToken(user,userRoles,Permission);
            var newRefreshToken = GenerateRefreshToken();
            var ExpirationDate = DateTime.UtcNow.AddDays(_RefreshTokenExpirationDate);

            user.RefreshTokens.Add(new RefreshTokens
            {
                Token = newRefreshToken,
                ExpireOn = ExpirationDate
            });
            await _UserManager.UpdateAsync(user);
            var result = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, Newtoken, expireIn, newRefreshToken, ExpirationDate);
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

        public async Task<Result> ForgetPasswordAsync(string Email)
        {
            var user = await _UserManager.FindByEmailAsync(Email);
            if (user is null)
                return Result.success();

            if (!user.EmailConfirmed)
                return Result.Failure(UserCredentials.EmailNotConfirmed);

            var code = await _UserManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            _logger.LogInformation("Confirmation code : {code}", code);

            await SendForgetPasswordEmail(user, code);

            return Result.success();

        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _UserManager.FindByEmailAsync(request.Email);
            if (user is null || !user.EmailConfirmed)
                return Result.Failure(UserCredentials.EmailNotConfirmed);

            IdentityResult result;

            try
            {
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
                result = await _UserManager.ResetPasswordAsync(user, code, request.NewPassword);

            }
            catch (FormatException)
            {
                return Result.Failure(UserCredentials.InvalidCode);
            }
            if (result.Succeeded)
                return Result.success();

            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        private async Task SendConfirmationEmail(ApplicationUser user,string code)
        {
            var Origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

            var EmailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation", new Dictionary<string, string>
                {
                    {"{{UserName}}",user.FirstName },
                    {"{{AppName}}" ,"Survey Basket"},
                    {"{{ConfirmationLink}}",$"{Origin}/auth/emailConfirmation?userId={user.Id}&code={code}" }
                });
            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "✅ Survey Basket : Email Confirmation", EmailBody));
            await Task.CompletedTask;
        }
        private async Task SendForgetPasswordEmail(ApplicationUser user,string code)
        {
            var Origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

            var EmailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword", new Dictionary<string, string>
                {
                    {"{{name}}",user.FirstName },
                    { "{{action_url}}", $"{Origin}/auth/forgetPassword?email={user.Email}&code={code}" }  
                }
            );
            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "✅ Survey Basket: Change Password ", EmailBody));
            await Task.CompletedTask;
        }
        private async Task <(IEnumerable<string> Roles,IEnumerable<string> Permission)> GetRolesAndPermission(ApplicationUser user,CancellationToken cancellationToken)
        {
            var userRoles = await _UserManager.GetRolesAsync(user);

            var Permission = await(from r in _context.Roles
                                   join p in _context.RoleClaims
                                   on r.Id equals p.RoleId
                                   where userRoles.Contains(r.Name!)
                                   select p.ClaimValue)
                                    .Distinct()
                                    .ToListAsync(cancellationToken);
            return (userRoles, Permission);
        }
    }
}
