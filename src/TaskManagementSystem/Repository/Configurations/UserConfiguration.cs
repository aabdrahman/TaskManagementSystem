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
            .OnDelete(DeleteBehavior.Cascade).IsRequired();
    }
}
