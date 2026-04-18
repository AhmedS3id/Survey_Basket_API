using Survey_Basket_API.Contract.Questions;

namespace Survey_Basket_API.Mapping
{
    public class Mapping_Confiq : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<RegisterRequest, ApplicationUser>()
              .Map(des => des.FirsName, src => src.FirstName);

            config.NewConfig<QuestionRequest, Question>()
                .Map(des => des.Answers, src => src.Answers.Select(Answer => new Answer { Content = Answer }));

            config.NewConfig<RegisterRequest,ApplicationUser>()
                .Map(des => des.UserName, src => src.Email);
        }
    }
}
