using Microsoft.AspNetCore.Identity;
namespace Survey_Basket_API.Services
{
    public class UserService(UserManager<ApplicationUser>userManager):IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task <Result<UsersProfileResponse>> GetProfileAsync(String Id)
        {
            //var user = await _userManager.FindByIdAsync(Id);

            var user = await _userManager.Users
                .Where(x => x.Id == Id)
                .ProjectToType<UsersProfileResponse>()
                .FirstAsync();

            return Result.success( user);
        }
        public async Task <Result> UpdateUserProfileAsync(string Id,UpdateProfileRequest request)
        {
            var user = await _userManager.Users
                .Where(x=>x.Id==Id)
                .ExecuteUpdateAsync(s => s
                     .SetProperty(u=>u.FirstName,request.FirstName)
                      .SetProperty(u => u.LastName, request.LastName));
            //user = request.Adapt(user);
            //await _userManager.UpdateAsync(user!);

            return Result.success();
        }
        public async Task <Result> ChangePasswordAsync(string Id,ChangePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(Id);
            var result = await _userManager.ChangePasswordAsync(user!, request.CurrentPassword,request.NewPassword);
            if (result.Succeeded) 
                return Result.success();

            var error=result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
    }
}
