using Survey_Basket_API.Contract.Roles;

namespace Survey_Basket_API.Services
{
    public interface IRoleServices
    {
        Task<IEnumerable<RolesResponse>> GetAllAsync(bool? IncludeDeleted = false, CancellationToken cancellationToken=default);
        Task<Result<RolesDetailResponse>> GetByIdAsync(string Id, CancellationToken cancellationToken = default);
        Task<Result<RolesDetailResponse>> AddAsync(RolesRequest request, CancellationToken cancellationToken);
        Task<Result> UpdateAsync(string Id, RolesRequest request, CancellationToken cancellationToken);
        Task<Result> ToggleStatus(string Id, CancellationToken cancellationToken);

    }
}
