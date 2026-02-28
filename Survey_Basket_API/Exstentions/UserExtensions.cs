using System.Security.Claims;

namespace Survey_Basket_API.Extensions
{
    public static class UserExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal User)
        {
            return( User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}
