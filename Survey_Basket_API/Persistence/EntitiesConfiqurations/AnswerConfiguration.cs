using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Survey_Basket_API.Persistence.EntitiesConfiguration
{
    public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.HasIndex(x => new {x.QuestionId,x.Content}).IsUnique();
            builder.Property(x => x.Content).HasMaxLength(1000);

        }
    }
}
