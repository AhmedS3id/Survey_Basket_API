using Microsoft.AspNetCore.Identity;

namespace Survey_Basket_API.Entities
{
    public sealed class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Id = Guid.CreateVersion7().ToString();
            SecurityStamp = Guid.CreateVersion7().ToString();
        }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsDisabled { get; set; }

        public List<RefreshTokens> RefreshTokens = [];
    }
}
