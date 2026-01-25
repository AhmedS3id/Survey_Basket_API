namespace Survey_Basket_API.Contract.Authentication
{
    public record AuthResponse(
        string id,
        string? Email,
        string FirstName,
        string LastName,
        string Token,
        int ExpiresIn,
        string RefreshToken,
        DateTime RefreshTokenExpiration
        );
    
}
