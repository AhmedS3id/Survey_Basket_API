using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Survey_Basket_API.Abstractions.Consts;
using Survey_Basket_API.Mapping;
using Survey_Basket_API.Persistence;
namespace Survey_Basket_API.Services
{
    public class UserService(UserManager<ApplicationUser>userManager,AppDbContext context ):IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<UserResponse>> GetAllAsync() =>
             await (from u in _context.Users
                    join ur in _context.UserRoles
                    on u.Id equals ur.UserId
                    join r in _context.Roles
                    on ur.RoleId equals r.Id
                    group r by new
                    {
                        u.Id,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        u.IsDisabled
                    } into g
                    where !g.Any(x => x.Name == DefaultRoles.Member)
                    select new UserResponse
                    (
                         g.Key.Id,
                         g.Key.FirstName,
                         g.Key.LastName,
                         g.Key.Email!,
                         g.Key.IsDisabled,
                         g.Select(x => x.Name!).ToList()
                    )).ToListAsync();
        //await (from u in _context.Users
        //       join ur in _context.UserRoles
        //       on u.Id equals ur.UserId
        //       join r in _context.Roles
        //       on ur.RoleId equals r.Id into roles
        //       where (!roles.Any(x => x.Name == DefaultRoles.Member))
        //       select new
        //       {
        //           u.Id,
        //           u.FirstName,
        //           u.LastName,
        //           u.Email,
        //           u.IsDisabled,
        //           Roles = roles.Select(roles => roles.Name!).ToList()
        //       }
        //       ).GroupBy(x => new {x.Id,x.FirstName,x.LastName,x.Email,x.IsDisabled})
        //        .Select (u=>new UserResponse(
        //            u.Key.Id,
        //            u.Key.FirstName,
        //            u.Key.LastName,
        //            u.Key.Email!,
        //            u.Key.IsDisabled,
        //            u.SelectMany(x =>x.Roles)
        //      )).ToListAsync();

        public async Task <Result<UserResponse>> AddAsync (CreateUserRequest request)
        {
            var isEmailExist = await _userManager.Users.AnyAsync(x=>x.Email==request.Email);
            if (isEmailExist)
                return Result.Failure<UserResponse>(UserCredentials.InvalidEmail);

            var ValidRolls = await _context.Roles.Select(x => x.Name).ToListAsync();

            if (request.Roles.Except(ValidRolls).Any())
                return Result.Failure<UserResponse>(RolesError.NotAllowedRoles);

            var user = request.Adapt<ApplicationUser>();
            user.UserName = request.Email;
            user.EmailConfirmed = true;
            var result = await _userManager.CreateAsync(user,request.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user,request.Roles);
                var response = new UserResponse(
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email!,
                    user.IsDisabled,
                    request.Roles
                    );
                return Result.success<UserResponse>(response);
            }
            var error = result.Errors.FirstOrDefault();
            return Result.Failure<UserResponse>(new Error(error!.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task <Result> UpdateAsync (string id,UpdateUserRequest request)
        {
            var isEmailExist = await _userManager.Users.AnyAsync(x=>x.Email==request.Email&&x.Id!=id);
            if (isEmailExist)
                return Result.Failure<UserResponse>(UserCredentials.InvalidEmail);

            var ValidRolls = await _context.Roles.Select(x => x.Name).ToListAsync();
            if (request.Roles.Except(ValidRolls).Any())
                return Result.Failure<UserResponse>(RolesError.NotAllowedRoles);

            if (await _userManager.FindByIdAsync(id) is not { } user)
                return Result.Failure<UserResponse>(UserCredentials.UserNotFound);

            user = request.Adapt(user);
            user.UserName = request.Email;
            user.NormalizedUserName = request.Email.ToUpper();

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _context.UserRoles
                    .Where(x=>x.UserId==id)
                    .ExecuteDeleteAsync();
                await _userManager.AddToRolesAsync(user,request.Roles);
                return Result.success();
            }
            var error = result.Errors.FirstOrDefault();
            return Result.Failure(new Error(error!.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<Result> ToggleStatus(string id)
        {
            if (await _userManager.FindByIdAsync(id) is not { } user)
                return Result.Failure(UserCredentials.UserNotFound);

            user.IsDisabled = !user.IsDisabled;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded) 
                return Result.success();

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code,error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<Result> UnlockAcc(string id)
        {
            if (await _userManager.FindByIdAsync(id) is not { } user)
                return Result.Failure(UserCredentials.UserNotFound);

            var result = await _userManager.SetLockoutEndDateAsync(user, null);

            if (result.Succeeded) 
                return Result.success();

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code,error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<Result<UserResponse>> GetAsync(string id)
        {
            if (await _userManager.FindByIdAsync(id) is not { } user)
                return Result.Failure<UserResponse>(UserCredentials.UserNotFound);
            var userRoles = await _userManager.GetRolesAsync(user);
            var Response = new UserResponse
            (
                 id,
                 user.FirstName,
                 user.LastName,
                 user.Email!,
                 user.IsDisabled,
                 userRoles
            );
            return Result<UserResponse>.success( Response );
        }
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
