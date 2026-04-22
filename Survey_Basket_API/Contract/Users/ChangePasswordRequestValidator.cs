using Survey_Basket_API.Abstractions.Const;

namespace Survey_Basket_API.Contract.Users
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x=>x.CurrentPassword)
                .NotEmpty();
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .Matches(RegexPattern.Password)
                .WithMessage("Password must be at least 8 digits and must contain LowerCase,UpperCase,Numbers,NonAlphabetic ")
                .NotEqual(x => x.CurrentPassword)
                .WithMessage("The new password must't not equal old password");
        }
    }
}
