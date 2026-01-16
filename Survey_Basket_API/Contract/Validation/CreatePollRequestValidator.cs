namespace Survey_Basket_API.Contract.Validation
    
{
    public class CreatePollRequestValidator:AbstractValidator<PollRequest>
    {
        public CreatePollRequestValidator()
        {
            RuleFor(x =>x.Title).NotEmpty()
                .Length(3,100); 

            RuleFor(x =>x.Summary).NotEmpty()
                .Length(3,100);

            RuleFor(x => x.StartsAt).NotEmpty()
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));

            RuleFor(x => x.EndsAt).NotEmpty();

            RuleFor(x => x).Must(HasValidDate)
                .WithName(nameof(PollRequest.EndsAt))//"Ends At"
                .WithMessage("The {PropertyName} Must be Greater Than Or Equal StartsAt");


        }
        private bool HasValidDate(PollRequest pollRequest)
        {
            return pollRequest.EndsAt>=pollRequest.StartsAt;
        }
    }
}
