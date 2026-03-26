namespace Survey_Basket_API.Contract.Results
{
    public record VotePerQuestionResponse(
        string Question ,
        IEnumerable<VotePerAnswerResponse> SelectedAnswer 
        );
    
}
