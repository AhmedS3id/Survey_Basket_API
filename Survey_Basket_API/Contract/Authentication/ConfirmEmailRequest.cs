namespace Survey_Basket_API.Contract.Authentication
{
    public record ConfirmEmailRequest(
        string UserId,
        string Code
        );
    
}
