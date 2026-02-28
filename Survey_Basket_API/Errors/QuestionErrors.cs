namespace Survey_Basket_API.Errors
{
    public class QuestionErrors
    {
        public static readonly Error QuestionNotFound =
            new("Question Not Found", "No Question Was Found With Given Id", StatusCodes.Status404NotFound);

        public static readonly Error DuplicatedQuestionContent=
            new("Question.Duplicated", "Another Question With The same Content", StatusCodes.Status409Conflict);
    }
}
