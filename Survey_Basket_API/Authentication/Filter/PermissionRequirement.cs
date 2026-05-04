namespace Survey_Basket_API.Authentication.Filter
{
    public class PermissionRequirement(string permission):IAuthorizationRequirement
    {
        public string Permission { get; } = permission;
    }
}
