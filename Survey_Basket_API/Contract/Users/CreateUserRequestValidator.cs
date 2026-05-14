using Survey_Basket_API.Abstractions.Const;

namespace Survey_Basket_API.Contract.Users
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress()
                .NotEmpty();
            RuleFor(x => x.Password)
                .NotEmpty()
                .Matches(RegexPattern.Password)
                .WithMessage("Password must be at least 8 digits and must contain LowerCase,UpperCase,Numbers,NonAlphabetic");
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .Length(3, 200);
            RuleFor(x => x.LastName)
                .NotEmpty()
                .Length(3, 200);

            RuleFor(x => x.Roles)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.Roles)
                .Must(x => x.Distinct().Count() == x.Count)
                .WithMessage("You can't duplicated roles to the same user")
                .When(x => x.Roles != null);
           // to avoid exception 
        }
    }
}
