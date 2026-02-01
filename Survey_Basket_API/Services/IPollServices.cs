using Survey_Basket_API.Entities;

namespace Survey_Basket_API.Services
{
    public  interface IPollServices
    {
        Task <IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken =default);

        Task <Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default);

        Task<PollResponse> AddAsync(PollRequest poll, CancellationToken cancellationToken = default);
        Task< Result >updateAsync(int id, PollRequest poll,CancellationToken cancellationToken=default);

        Task<Result> deleteAsync(int id, CancellationToken cancellationToken = default);
        Task<Result> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default);
    }
}
