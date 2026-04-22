using Survey_Basket_API.Contract.Users;

namespace Survey_Basket_API.Services
{
    public interface IUserService
    {
         Task<Result<UsersProfileResponse>> GetProfileAsync(String Id);
         Task<Result> UpdateUserProfileAsync(string Id, UpdateProfileRequest request);
        Task<Result> ChangePasswordAsync(string Id, ChangePasswordRequest request);


    }
}
