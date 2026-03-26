namespace Survey_Basket_API.Contract.Results
{
    public record VotePerDayResponse(
        DateOnly Date,
        int NumberOfVotes
        );
    
    
}
