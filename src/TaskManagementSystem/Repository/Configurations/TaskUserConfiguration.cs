using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

internal class TaskUserConfiguration : IEntityTypeConfiguration<TaskUser>
{
    public void Configure(EntityTypeBuilder<TaskUser> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Id);

        builder.HasIndex(x => x.TaskId);

        builder.HasIndex(x => x.UserId);

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.ProposedCompletionDate)
            .IsRequired()
            .HasColumnType("date");

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.user)
            .WithMany(x => x.TaskLink)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade).IsRequired();
        builder.HasOne(x => x.task)
            .WithMany(x => x.UserLink)
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Cascade).IsRequired();
    }
}
