namespace Survey_Basket_API.Contract.Authentication
{
    public class ForgetPasswordRequestValidator : AbstractValidator<ForgetPasswordRequest>
    {
        public ForgetPasswordRequestValidator()
        {
            RuleFor(x => x.Email).
                NotEmpty().
                EmailAddress();
        }
    }
}
