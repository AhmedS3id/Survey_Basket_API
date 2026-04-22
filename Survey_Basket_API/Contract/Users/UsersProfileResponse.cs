 namespace Survey_Basket_API.Contract.Users
{
    public record UsersProfileResponse(
        string Email,
        string UserName,
        string FirstName,
        string LastName
        );
}
