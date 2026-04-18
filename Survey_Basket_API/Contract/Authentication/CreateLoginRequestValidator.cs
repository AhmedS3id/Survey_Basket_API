namespace Survey_Basket_API.Contract.Auth
    
{
    public class CreateLoginRequestValidator:AbstractValidator<LoginRequest>
    {
        public CreateLoginRequestValidator()
        {

            RuleFor(x => x.Email).NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password).NotEmpty();

        }
      
    }
}
