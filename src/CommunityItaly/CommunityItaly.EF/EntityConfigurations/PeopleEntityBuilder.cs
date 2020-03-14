using CommunityItaly.EF.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunityItaly.EF.EntityConfigurations
{
    public class PersonEntityBuilder : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToContainer(nameof(Person))
                .HasNoDiscriminator();
            builder.HasKey(x => x.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();

            builder.Property(x => x.Name);
            builder.Property(x => x.Surname);
        }
    }
}
