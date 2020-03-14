﻿using CommunityItaly.EF.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityItaly.EF.EntityConfigurations
{
    public class CommunityEntityBuilder : IEntityTypeConfiguration<Community>
    {
        public void Configure(EntityTypeBuilder<Community> builder)
        {
            builder.ToContainer(nameof(Community))
                .HasNoDiscriminator();
            builder.HasKey(x => x.Name);


            builder.Property(x => x.Name);
            builder.HasMany(x => x.Managers);
        }
    }
}
