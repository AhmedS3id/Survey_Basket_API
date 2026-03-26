namespace Survey_Basket_API.Contract.Results
{
    public record VoteResponse(
    string VoterName,
    DateTime VoteDate,
    IEnumerable<QuestionAnswerResponse>SelectedAnswers
        );
}
