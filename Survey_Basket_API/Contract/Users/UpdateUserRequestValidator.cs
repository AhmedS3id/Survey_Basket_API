namespace Survey_Basket_API.Contract.Users
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress()
                .NotEmpty();
           
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
        }
    }
}
