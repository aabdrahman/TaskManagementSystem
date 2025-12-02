using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Id);

        builder.HasIndex(x => x.UnitId);

        builder.HasIndex(x => x.Username)
            .IsUnique();

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.HasQueryFilter(x => x.IsActive);

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.FirstName)
            .HasMaxLength(50) 
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Password)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(75)
            .IsRequired();

        builder.Property(x => x.Username)
            .HasMaxLength(75)
            .IsRequired();
        builder.Property(x => x.LastLoginDate)
            .IsRequired(false);
        builder.Property(x => x.LastPasswordChangeDate)
            .IsRequired(true)
            .HasDefaultValueSql("getdate()");

        builder.HasOne(x => x.AssignedUnit)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.UnitId)   
            .OnDelete(DeleteBehavior.Cascade).IsRequired();

        builder.HasMany(x => x.RoleLink)
            .WithOne(x => x.user)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade).IsRequired();

        builder.HasMany(x => x.TaskLink)
            .WithOne(x => x.user)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

    }
}
