using CommunityItaly.EF.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Text.Json;

namespace CommunityItaly.EF.EntityConfigurations
{
	public class CommunityEntityBuilder : IEntityTypeConfiguration<Community>
    {
        public void Configure(EntityTypeBuilder<Community> builder)
        {
            builder.ToContainer(nameof(Community))
                .HasNoDiscriminator();
            builder.HasKey(x => x.ShortName);


            builder.Property(x => x.ShortName);
            builder.Property(x => x.ManagerCollection)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions()));
            //builder.HasMany(x => x.ManagerCollection);
            builder.Ignore(x => x.Managers);
        }
    }
}
