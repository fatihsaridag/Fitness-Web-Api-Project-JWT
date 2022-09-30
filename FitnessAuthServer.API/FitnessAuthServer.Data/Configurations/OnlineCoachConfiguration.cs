using FitnessAuthSever.Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessAuthServer.Data.Configurations
{
    public class OnlineCoachConfiguration : IEntityTypeConfiguration<OnlineCoach>
    {
        public void Configure(EntityTypeBuilder<OnlineCoach> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Name).HasMaxLength(200);
            builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Stock).IsRequired();
            builder.Property(x => x.UserId).IsRequired();
        }
    }
}
