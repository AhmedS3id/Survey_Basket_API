namespace Survey_Basket_API.Contract.Authentication
{
    public record ResetPasswordRequest(
        string Email,
        string Code,
        string NewPassword 
        );

    
}
