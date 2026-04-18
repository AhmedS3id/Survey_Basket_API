using Survey_Basket_API.Abstractions.Const;

namespace Survey_Basket_API.Contract.Authentication
{
    public class ResendConfirmationEmailRequestValidator : AbstractValidator<ResendConfirmationEmailRequest>
    {
        public ResendConfirmationEmailRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty()
                .EmailAddress();

        }
    }
}