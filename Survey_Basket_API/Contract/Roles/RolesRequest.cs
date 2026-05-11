namespace Survey_Basket_API.Contract.Roles
{
    public record RolesRequest
    (
        string Name,
        IList<string> Permissions
        );
}
