using CommunityItaly.EF.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityItaly.EF.EntityConfigurations
{
    public class ArticleEntityBuilder : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.ToContainer(nameof(Article))
               .HasNoDiscriminator()
               .HasPartitionKey(o => o.Id);
            builder.HasKey(x => x.Id);

            builder.HasMany(x => x.Authors);
        }
    }
}
