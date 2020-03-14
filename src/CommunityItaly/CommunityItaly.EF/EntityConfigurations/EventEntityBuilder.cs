using CommunityItaly.EF.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunityItaly.EF.EntityConfigurations
{
    public class EventEntityBuilder : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToContainer(nameof(Event))
                .HasNoDiscriminator();
            builder.HasKey(x => x.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();

            builder.Property(x => x.Name);
            builder.Property(x => x.StartDate);
            builder.Property(x => x.EndDate);

            builder.OwnsOne(x => x.CFP,
                c =>
                {
                    c.HasKey("Url");
                    c.Property(t => t.Url);
                    c.Property(t => t.StartDate);
                    c.Property(t => t.EndDate);
                });

            builder.HasOne(x => x.Community);
        }
    }
}
