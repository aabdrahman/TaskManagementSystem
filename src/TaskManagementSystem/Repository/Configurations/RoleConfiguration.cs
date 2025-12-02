using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

internal class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Id);

        builder.Property(x => x.Id).IsRequired();   

        builder.Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.NormalizedName)
            .HasComputedColumnSql("UPPER([Name])")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasMany(x => x.UserLink)
            .WithOne(x => x.role)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade).IsRequired();
    }
}
