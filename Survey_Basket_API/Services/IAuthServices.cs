
namespace Survey_Basket_API.Services
{
    public interface IAuthServices
    {
        Task<AuthResponse?> GetTokenAsync (string email,String Password,CancellationToken cancellationToken);
        Task<AuthResponse?> GetRefreshTokenAsync (string Token,String RefreshToken,CancellationToken cancellationToken);
        Task <bool> InvokeTokenAsync (string Token,String RefreshToken,CancellationToken cancellationToken);
    }
}
