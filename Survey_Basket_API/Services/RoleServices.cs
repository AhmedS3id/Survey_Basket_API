using Microsoft.AspNetCore.Identity;
using Survey_Basket_API.Abstractions.Consts;
using Survey_Basket_API.Contract.Roles;
using Survey_Basket_API.Persistence;

namespace Survey_Basket_API.Services
{
    public class RoleServices(AppDbContext context,
        RoleManager<ApplicationRole> roleManager
        ) : IRoleServices
    {
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<RolesResponse>> GetAllAsync(bool? IncludeDisable = false,CancellationToken cancellationToken=default) =>
            await _roleManager.Roles
            .Where(x => !x.IsDefault && (!x.IsDeleted || IncludeDisable.HasValue && IncludeDisable.Value))
            .ProjectToType<RolesResponse>()
            .ToListAsync(cancellationToken);
        public async Task<Result<RolesDetailResponse>> GetByIdAsync(string Id, CancellationToken cancellationToken = default)
        {
            if(await  _roleManager.FindByIdAsync(Id) is not { } Role)
                return Result.Failure<RolesDetailResponse>(RolesError.InvalidRoles);
            var Permission = await _roleManager.GetClaimsAsync(Role);
            var Response = new RolesDetailResponse(Id, Role.Name!, Role.IsDeleted, Permission.Select(x => x.Value));
            return Result.success(Response);
        }
        public async Task<Result<RolesDetailResponse>> AddAsync(RolesRequest request, CancellationToken cancellationToken)
        {
            var roleIsExist = await _roleManager.RoleExistsAsync(request.Name);
            if (roleIsExist)
                return Result.Failure<RolesDetailResponse>(RolesError.DuplicatedRole);
            var AllowedPermissions = Permissions.GetAllPermissions();

            if (request.Permissions.Except(AllowedPermissions).Any() )
                return Result.Failure<RolesDetailResponse>(RolesError.NoPermission);

            var role = new ApplicationRole
            {
                Name = request.Name,
                ConcurrencyStamp=Guid.NewGuid().ToString()
            };
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                var permissions = request.Permissions
                    .Select(x => new IdentityRoleClaim<string>
                    {
                        ClaimType = Permissions.Type,
                        ClaimValue = x,
                        RoleId = role.Id
                    });
                await _context.AddRangeAsync(permissions, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                var response = new RolesDetailResponse(role.Id, role.Name,role.IsDeleted,request.Permissions);
                return Result.success(response);
            }
               var error = result.Errors.FirstOrDefault();
            return Result.Failure<RolesDetailResponse>(new Error(error!.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task <Result> UpdateAsync(string Id , RolesRequest request, CancellationToken cancellationToken)
        {
            var roleIsExist = await _roleManager.Roles.AnyAsync(x => x.Name == request.Name && x.Id == Id, cancellationToken: cancellationToken);
            if (!roleIsExist)
                return Result.Failure(RolesError.DuplicatedRole);

            if (await _roleManager.FindByIdAsync(Id) is not{ }role)
                return Result.Failure(RolesError.InvalidRoles);

            var AllowedPermissions = Permissions.GetAllPermissions();

            if (request.Permissions.Except(AllowedPermissions).Any())
                return Result.Failure(RolesError.NoPermission);

            role.Name=request.Name;

           var result= await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                var currentPermission = await _context.RoleClaims
                     .Where(x => x.RoleId == Id)
                     .Select(x => x.ClaimValue)
                     .ToListAsync(cancellationToken: cancellationToken);
                var newPermissions = request.Permissions.Except (currentPermission).Select(x => new IdentityRoleClaim<string>
                {
                    ClaimType = Permissions.Type,
                    ClaimValue = x,
                    RoleId = role.Id
                }); 
                   
                var removedPermissions = currentPermission.Except(request.Permissions);

                await _context.RoleClaims
                    .Where(x => x.RoleId == Id && removedPermissions.Contains(x.ClaimValue))
                    .ExecuteDeleteAsync(cancellationToken: cancellationToken);

                await _context.AddRangeAsync(newPermissions, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return Result.success();
            }
            var error = result.Errors.FirstOrDefault();
            return Result.Failure(new Error(error!.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        public async Task<Result> ToggleStatus(string Id, CancellationToken cancellationToken)
        {
            if (await _roleManager.FindByIdAsync(Id) is not { } role)
                return Result.Failure(RolesError.InvalidRoles);
            role.IsDeleted = !role.IsDeleted;

            await _context.SaveChangesAsync(cancellationToken);
            return Result.success();
        }

    }

}
