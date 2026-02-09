using Survey_Basket_API.Contract.Questions;

namespace Survey_Basket_API.Services
{
    public interface IQuestionServices
    {
        Task<Result<QuestionResponse>> AddAsync(int PollId, QuestionRequest request, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int PollId,CancellationToken cancellationToken = default);
        Task<Result<QuestionResponse>> GetAsync(int PollId, int id, CancellationToken cancellationToken = default);
        Task<Result> ToggleStatusAsync(int PollId, int id, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(int PollId, int id, QuestionRequest request, CancellationToken cancellationToken = default);

    }
}
