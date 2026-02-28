namespace Survey_Basket_API.Contract.Votes
{
    public record VoteRequest(
         ICollection<VoteAnswerRequest> Answer
        );
    
    
}
