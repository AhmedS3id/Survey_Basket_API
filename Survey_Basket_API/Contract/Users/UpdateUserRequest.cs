namespace Survey_Basket_API.Contract.Users
{
    public record UpdateUserRequest(
       string FirstName,
        string LastName,
        string Email,
        IList<string>Roles
        );
    
}
