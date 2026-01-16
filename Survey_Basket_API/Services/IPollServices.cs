using Survey_Basket_API.Entities;

namespace Survey_Basket_API.Services
{
    public  interface IPollServices
    {
        Task <IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken =default);

        Task < Poll?> GetAsync(int id, CancellationToken cancellationToken = default);

        Task < Poll> AddAsync(Poll poll, CancellationToken cancellationToken = default);
         Task< bool >updateAsync(int id, Poll poll,CancellationToken cancellationToken=default);

        Task< bool> deleteAsync(int id,CancellationToken cancellationToken=default);
        Task< bool> TogglePublishStatusAsync(int id,CancellationToken cancellationToken=default);
    }
}
