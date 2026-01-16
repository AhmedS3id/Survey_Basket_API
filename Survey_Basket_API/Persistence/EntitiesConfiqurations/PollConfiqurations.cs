using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Survey_Basket_API.Persistence.EntitiesConfiqurations
{
    public class PollConfiqurations : IEntityTypeConfiguration<Poll>
    {
        public void Configure(EntityTypeBuilder<Poll> builder)
        {
            builder.HasIndex(x => x.Title).IsUnique();
            builder.Property(x => x.Title).HasMaxLength(1000);
            builder.Property(x => x.Summary).HasMaxLength(1500);

        }
    }
}
