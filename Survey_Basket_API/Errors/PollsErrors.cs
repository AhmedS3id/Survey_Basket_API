namespace Survey_Basket_API.Errors
{
    public static class PollsErrors
    {
        
      public static readonly Error InvalidPolls = new("Polls Not Found", "No Poll was found With given id",StatusCodes.Status404NotFound);
      public static readonly Error DuplicatedTitle = new("Duplicated Title", "Another Polls With The same title", StatusCodes.Status409Conflict);
        
    }
}
