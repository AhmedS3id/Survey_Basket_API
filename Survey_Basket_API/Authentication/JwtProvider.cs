
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Survey_Basket_API.Authentication
{
    public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
    {
        private readonly JwtOptions _options  = options.Value;

        public (string token, int expireIn) GenerateToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions)
        {
            Claim[] claims = [
                new(JwtRegisteredClaimNames.Sub,user.Id),
                new(JwtRegisteredClaimNames.Email,user.Email!),
                new(JwtRegisteredClaimNames.GivenName,user.FirstName),
                new(JwtRegisteredClaimNames.FamilyName,user.LastName),
                new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new(nameof(roles),JsonSerializer.Serialize(roles),JsonClaimValueTypes.JsonArray),
                new(nameof(permissions),JsonSerializer.Serialize(permissions),JsonClaimValueTypes.JsonArray)
                ];
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
            var SigningCredentials = new SigningCredentials(symmetricSecurityKey,SecurityAlgorithms.HmacSha256 );
           //var expiresIn = 30;
 
            var token = new JwtSecurityToken(
                issuer:_options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.ExpireMinutes),
                signingCredentials: SigningCredentials
                );
            return (token: new JwtSecurityTokenHandler().WriteToken(token), _options.ExpireMinutes * 60);
        }

        public string? ValidateToken(string token) =>
            ValidateTokenCore(token, validateLifetime: true);

        public string? ValidateExpiredToken(string token) =>
            ValidateTokenCore(token, validateLifetime: false);

        private string? ValidateTokenCore(string token, bool validateLifetime)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    IssuerSigningKey = symmetricSecurityKey,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidIssuer = _options.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _options.Audience,
                    ValidateLifetime = validateLifetime,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                return jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
