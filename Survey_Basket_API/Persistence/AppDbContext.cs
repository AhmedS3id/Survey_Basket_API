

using Survey_Basket_API.Persistence.EntitiesConfiqurations;
using System.Reflection;

namespace Survey_Basket_API.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options ) : DbContext(options )
    {
        public DbSet<Poll>Polls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

    }
}
