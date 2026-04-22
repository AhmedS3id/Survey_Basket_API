namespace Survey_Basket_API.Contract.Users
{
    public record ChangePasswordRequest(
        string CurrentPassword,
        string NewPassword
        );

    
}
