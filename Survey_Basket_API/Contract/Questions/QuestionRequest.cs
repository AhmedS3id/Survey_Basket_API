namespace Survey_Basket_API.Contract.Questions
{
    public record QuestionRequest
    (
        String Content,
        List <String>Answers
    );
}
