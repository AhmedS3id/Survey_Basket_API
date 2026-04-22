using Microsoft.AspNetCore.Identity;

namespace Survey_Basket_API.Entities
{
    public sealed class ApplicationUser :IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public List<RefreshTokens> RefreshTokens = [];
    }
}
