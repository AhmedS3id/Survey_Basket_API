namespace Survey_Basket_API.Abstractions.Const
{
    public static class RegexPattern
    {
       public const string Password = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$";
    }
}
