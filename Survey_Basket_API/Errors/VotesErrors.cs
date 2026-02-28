namespace Survey_Basket_API.Errors
{
    public static class VotesErrors
    {
        
      public static readonly Error InvalidQuestion = new("Vote.InvalidQuestion ", "Invalid Question ", StatusCodes.Status400BadRequest);
      public static readonly Error DuplicatedVote = new("Duplicated votes", "This user assign This vote before ", StatusCodes.Status409Conflict);
        
    }
}
