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

            builder.OwnsOne(x => x.Community,
             c =>
             {
                 c.HasKey("ShortName");
                 c.Property(t => t.Name);
                 c.Property(t => t.WebSite);
                 c.Property(t => t.Logo);
                 c.OwnsMany(f => f.Managers,
                     k =>
                     {
                         k.HasKey("Id");
                         k.Property(t => t.Name);
                         k.Property(t => t.Surname);
                         k.Property(t => t.Picture);
                         k.Property(t => t.MVP_Code);
                     });
             });
        }
    }
}
