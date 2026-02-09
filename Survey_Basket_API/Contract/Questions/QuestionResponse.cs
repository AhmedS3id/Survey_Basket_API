using Survey_Basket_API.Contract.Answers;

namespace Survey_Basket_API.Contract.Questions
{
    public record QuestionResponse
    (
        int Id ,
        string Content,
        IEnumerable<AnswerResponse> Answers
        );
}
