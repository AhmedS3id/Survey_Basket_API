namespace Survey_Basket_API.Contract.Users
{
    public record CreateUserRequest(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        IList<string>Roles
        );
    
}
