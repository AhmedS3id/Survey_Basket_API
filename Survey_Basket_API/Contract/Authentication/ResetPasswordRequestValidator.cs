using Survey_Basket_API.Abstractions.Const;

namespace Survey_Basket_API.Contract.Authentication
{
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Code)
                .NotEmpty();

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .Matches(RegexPattern.Password)
                .WithMessage("Password must be at least 8 digits and must contain LowerCase,UpperCase,Numbers,NonAlphabetic ");



        }
    }
}
