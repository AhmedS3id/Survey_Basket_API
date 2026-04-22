using Survey_Basket_API.Abstractions.Const;

namespace Survey_Basket_API.Contract.Authentication
{
    public class UpdateUserProfileUserValidator : AbstractValidator<RegisterRequest>
    {
        public UpdateUserProfileUserValidator()
        {
            RuleFor(x=>x.FirstName).NotEmpty()
                .Length(3,100);

            RuleFor(x=>x.LastName).NotEmpty()
                .Length(3,100);

            RuleFor(x => x.Email).NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password).NotEmpty()
                .Matches(RegexPattern.Password)
                .WithMessage( "Password must be at least 8 digits and must contain LowerCase,UpperCase,Numbers,NonAlphabetic ");

        }
    }
}