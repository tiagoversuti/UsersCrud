using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Data.Mappings
{
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users")
                .HasKey(u => u.Id);

            builder.Property(u => u.Name)
                .IsRequired()
                .HasColumnType("varchar(200)");

            builder.Property(u => u.Login)
                .IsRequired()
                .HasColumnType("varchar(50)");

            builder.Property(u => u.Password)
                .IsRequired()
                .HasColumnType("varchar(250)");
        }
    }
}
