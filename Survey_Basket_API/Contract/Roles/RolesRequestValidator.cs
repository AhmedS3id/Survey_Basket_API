namespace Survey_Basket_API.Contract.Roles
{
    public class RolesRequestValidator : AbstractValidator<RolesRequest>
    {
        public RolesRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(3, 200);

            RuleFor(x => x.Permissions)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.Permissions)
                .Must(x => x.Distinct().Count() == x.Count)
                .WithMessage("You can't add duplicated permission to the same role")
                .When(x => x.Permissions != null);

        }
    }
}
