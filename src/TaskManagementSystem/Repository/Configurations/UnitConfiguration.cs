using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.NormalizedName)
            .IsRequired();

        builder.Property(x => x.UnitHeadName)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Id);

        builder.HasMany(x => x.Users)
            .WithOne(x => x.AssignedUnit)
            .HasForeignKey(x => x.UnitId)
            .OnDelete(DeleteBehavior.Cascade).IsRequired();

        builder.Property(x => x.NormalizedName)
            .HasComputedColumnSql("UPPER([Name])");
    }
}
