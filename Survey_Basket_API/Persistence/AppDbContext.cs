


using Survey_Basket_API.Persistence.EntitiesConfiguration;
using System.Reflection;
using System.Security.Claims;

namespace Survey_Basket_API.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options,IHttpContextAccessor httpContextAccessor ) 
        : IdentityDbContext<ApplicationUser>(options )
    {
        public DbSet<Poll>Polls { get; set; }
        public DbSet<Answer>Answers { get; set; }
        public DbSet<Question>Questions { get; set; }
        public DbSet<Vote>Votes { get; set; }
        public DbSet<VoteAnswer> VoteAnswers { get; set; }
        public IHttpContextAccessor _HttpContextAccessor  = httpContextAccessor;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            var CascadeFk = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade&&!fk.IsOwnership);
            foreach(var fk in CascadeFk)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            base.OnModelCreating(modelBuilder);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var CurrentUserId = _HttpContextAccessor.HttpContext?.User.GetUserId();
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
