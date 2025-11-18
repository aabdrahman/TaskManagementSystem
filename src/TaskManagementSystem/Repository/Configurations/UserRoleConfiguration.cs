using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

internal class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Id);

        builder.HasIndex(x => x.RoleId);

        builder.HasIndex(x => x.UserId);

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(x => x.role)
            .WithMany(x => x.UserLink)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade).IsRequired();

        builder.HasOne(x => x.user)
            .WithMany(x => x.RoleLink)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade).IsRequired();
    }
}
