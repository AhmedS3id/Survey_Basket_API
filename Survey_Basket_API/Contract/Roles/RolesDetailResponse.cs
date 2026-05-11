namespace Survey_Basket_API.Contract.Roles
{
    public record RolesDetailResponse(
        string Id,
        string Name,
        bool IsDeleted,
        IEnumerable<string>Permissions
        );
}
