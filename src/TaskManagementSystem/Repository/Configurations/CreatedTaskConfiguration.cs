using Entities.Models;
using Entities.StaticValues;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

internal class CreatedTaskConfiguration : IEntityTypeConfiguration<CreatedTask>
{
    public void Configure(EntityTypeBuilder<CreatedTask> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Id);

        builder.HasIndex(x => x.Priority);

        builder.HasIndex(x => x.TaskStage);

        builder.HasIndex(x => x.TaskId)
            .IsUnique();

        builder.ToTable(table => table.HasCheckConstraint("CK_Projected_Completion_Date", "ProjectedCompletionDate > CAST(GETDATE() AS DATE) OR [TaskStage] != 'Cancelled' "));

        builder.ToTable(table => table.HasCheckConstraint("CK_Priority_Level", $"[Priority] IN ('Low', 'Medium', 'High', 'Critical')"));

        builder.ToTable(table => table.HasCheckConstraint("CK_Stage", "[TaskStage] IN ('Development', 'Testing', 'Deployment', 'ChangeManagement', 'Completed', 'Cancelled', 'Review')"));

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.CancelReason)
            .HasMaxLength(250);

        builder.Property(x => x.ProjectedCompletionDate)
            .HasColumnType("date");

        builder.Property(x => x.Title)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired();

        builder.Property(x => x.Priority)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.TaskStage)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.TaskId)
            .HasComputedColumnSql("RIGHT(REPLICATE('0', 6) + [Id], 8)");

        builder.HasMany(x => x.UserLink)
            .WithOne(x => x.task)
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne(x => x.CreatedByUser)
            .WithMany(x => x.CreatedTasks)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
