
using Microsoft.AspNetCore.Identity;
using Survey_Basket_API.Errors;
using System.Security.Cryptography;

namespace Survey_Basket_API.Services
{
    public class AuthServices(UserManager<ApplicationUser> UserManager, IJwtProvider jwtProvider) : IAuthServices
    {
        private readonly UserManager<ApplicationUser> _UserManager = UserManager;
        private readonly IJwtProvider _jwtProvider = jwtProvider;
        private readonly int _RefreshTokenExpirationDate = 14;
        public async Task<Result<AuthResponse>> GetTokenAsync(string email, String Password, CancellationToken cancellationToken)
        {
            var user = await _UserManager.FindByEmailAsync(email);

            if (user is null)
                return (Result.Failure<AuthResponse>(UserCredentials.InvalidCredentials));

            var IsValidPassword = await _UserManager.CheckPasswordAsync(user, Password);

            if (!IsValidPassword)
                return (Result.Failure<AuthResponse>(UserCredentials.InvalidCredentials));


            var (token, expireIn) = _jwtProvider.GenerateToken(user);
            var RefreshToken = GenerateRefreshToken();
            var ExpirationDate = DateTime.UtcNow.AddDays(_RefreshTokenExpirationDate);

            user.RefreshTokens.Add(new RefreshTokens
            {
                Token = RefreshToken,
                ExpireOn = ExpirationDate
            });
            await _UserManager.UpdateAsync(user);
            var result = new AuthResponse(user.Id, user.Email, user.FirsName, user.LastName, token, expireIn, RefreshToken, ExpirationDate);
            return Result.success(result);
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
                return Result.Failure<AuthResponse>(UserCredentials.InvalidJwtToken));

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
                return Result.Failure<AuthResponse>(UserCredentials.in);

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
    }
}
