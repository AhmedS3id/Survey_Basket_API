namespace Survey_Basket_API.Contract.Votes
{
    public class VoteRequestValidator:AbstractValidator<VoteRequest>
    {
        public VoteRequestValidator()
        {
            RuleFor(x=>x.Answer)
                .NotEmpty();
            RuleForEach(x=>x.Answer).SetInheritanceValidator(y=>y.Add(new VoteAnswerRequestValidator()));
        }
    }
}
