
namespace Survey_Basket_API.Services
{
    public interface IAuthServices
    {
        Task<Result<AuthResponse>> GetTokenAsync (string email,String Password,CancellationToken cancellationToken);
        Task<Result<AuthResponse>> GetRefreshTokenAsync (string Token,String RefreshToken,CancellationToken cancellationToken);
        Task <Result> InvokeTokenAsync (string Token,String RefreshToken,CancellationToken cancellationToken);
        Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
        Task<Result> ConfirmationEmail(ConfirmEmailRequest request);
        Task<Result> ResendConfirmationEmail(ResendConfirmationEmailRequest request);
        Task<Result> ForgetPasswordAsync(string Email);
        Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
    }
}
