namespace Survey_Basket_API.Contract.Results
{
    public record PollVoteResponse(
        string Title,
        IEnumerable<VoteResponse> Votes
        );
}
