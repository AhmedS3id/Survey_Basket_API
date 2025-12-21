namespace Survey_Basket_API.Contract.Validation
    
{
    public class CreatePollRequestValidator:AbstractValidator<PollRequest>
    {
        public CreatePollRequestValidator()
        {
            RuleFor(x =>x.Title).NotEmpty()
                .Length(3,10); 

            RuleFor(x =>x.Description).NotEmpty()
                .Length(3,10); 
        }
    }
}
