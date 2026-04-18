namespace Survey_Basket_API.Contract.Authentication
{
    public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
    {
        public ConfirmEmailRequestValidator()
        {
            RuleFor(x=>x.UserId).NotEmpty();
            RuleFor(x=>x.Code).NotEmpty();

        }
    }
}
