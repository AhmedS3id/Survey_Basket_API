using Survey_Basket_API.Contract.Questions;

namespace Survey_Basket_API.Mapping
{
    public class Mapping_Confiq : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<QuestionRequest, Question>()
                .Map(des => des.Answers, src => src.Answers.Select(Answer => new Answer { Content = Answer }));
        }
    }
}
