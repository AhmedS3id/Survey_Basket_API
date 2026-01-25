namespace Survey_Basket_API.Authentication
{
    public interface IJwtProvider
    {
        (string token, int expireIn) GenerateToken(ApplicationUser user);
        string? ValidateToken(string token);
    }
}
