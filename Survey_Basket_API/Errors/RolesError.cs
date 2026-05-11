namespace Survey_Basket_API.Errors
{
    public static class RolesError
    {
        
      public static readonly Error InvalidRoles = new("Roles Not Found",
          "No Roles was found With given id",StatusCodes.Status404NotFound);

      public static readonly Error DuplicatedRole = new("Duplicated Roles",
          "Roles was found With same name",StatusCodes.Status400BadRequest);

      public static readonly Error NoPermission  = new("Permission Not Allowed",
          " Permission not allowed ", StatusCodes.Status400BadRequest);
        
    }
}
