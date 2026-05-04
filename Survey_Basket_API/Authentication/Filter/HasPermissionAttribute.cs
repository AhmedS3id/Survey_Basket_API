namespace Survey_Basket_API.Authentication.Filter
{
    public class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission)
    {
    }
}
