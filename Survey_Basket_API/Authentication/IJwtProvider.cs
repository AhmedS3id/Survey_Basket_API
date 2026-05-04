namespace Survey_Basket_API.Authentication
{
    public interface IJwtProvider
    {
        (string token, int expireIn) GenerateToken(ApplicationUser user,IEnumerable<string>Roles,IEnumerable<string>Permission );
        string? ValidateToken(string token);
    }
}
