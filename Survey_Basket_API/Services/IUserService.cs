using Survey_Basket_API.Contract.Users;

namespace Survey_Basket_API.Services
{
    public interface IUserService
    {
        Task<Result<UsersProfileResponse>> GetProfileAsync(String Id);
        Task<Result> UpdateUserProfileAsync(string Id, UpdateProfileRequest request);
        Task<Result> ChangePasswordAsync(string Id, ChangePasswordRequest request);
        Task<Result<UserResponse>> AddAsync(CreateUserRequest request);
        Task<Result> UpdateAsync(string id, UpdateUserRequest request);
        Task<Result<UserResponse>> GetAsync(string id);
        Task<Result> ToggleStatus(string id);
        Task<Result> UnlockAcc(string id);
        Task<IEnumerable<UserResponse>> GetAllAsync();

    }
}
