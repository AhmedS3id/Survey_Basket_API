namespace Survey_Basket_API.Contract.Questions
{
    public class QuestionRequestValidator:AbstractValidator<QuestionRequest>    
    {
        public QuestionRequestValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty()
                .Length(3, 100);

            RuleFor(x => x.Answers)
                .NotNull();

            RuleFor(x => x.Answers)
                .Must(x => x.Count > 1)
                .WithMessage("Question should has at least 2 answer")
                .When(x=>x.Answers!=null);

            RuleFor(x => x.Answers)
               .Must(x => x.Distinct().Count()==x.Count)
               .WithMessage("You cant add duplicated answer to the same question")
               .When(x => x.Answers != null);


        }
    }
}
