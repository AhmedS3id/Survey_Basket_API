
namespace Survey_Basket_API.Services
{
    public interface IAuthServices
    {
        Task<Result<AuthResponse>> GetTokenAsync (string email,String Password,CancellationToken cancellationToken);
        Task<Result<AuthResponse>> GetRefreshTokenAsync (string Token,String RefreshToken,CancellationToken cancellationToken);
        Task <Result> InvokeTokenAsync (string Token,String RefreshToken,CancellationToken cancellationToken);
    }
}
