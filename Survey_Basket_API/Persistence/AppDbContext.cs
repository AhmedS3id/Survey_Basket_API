


using Survey_Basket_API.Persistence.EntitiesConfiguration;
using System.Reflection;
using System.Security.Claims;

namespace Survey_Basket_API.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options,IHttpContextAccessor httpContextAccessor ) 
        : IdentityDbContext<ApplicationUser>(options )
    {
        public DbSet<Poll>Polls { get; set; }
        public IHttpContextAccessor _HttpContextAccessor  = httpContextAccessor;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var CurrentUserId = _HttpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var entries = ChangeTracker.Entries<AuditTableEntity>();
            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added) 
                {
                    entityEntry.Property(x => x.CreatedById).CurrentValue = CurrentUserId!;

                }
                else if (entityEntry.State == EntityState.Modified)
                {
                    entityEntry.Property(x => x.UpdatedById).CurrentValue = CurrentUserId;
                    entityEntry.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

    }
}
