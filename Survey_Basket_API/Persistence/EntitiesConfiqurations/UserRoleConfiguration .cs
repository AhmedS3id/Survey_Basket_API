using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Survey_Basket_API.Abstractions.Consts;

namespace Survey_Basket_API.Persistence.EntitiesConfiguration
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(new IdentityUserRole<string>
            {
                RoleId=DefaultRoles.AdminRoleId,
                UserId=DefaultUsers.AdminId
            });

        }
    }
}
