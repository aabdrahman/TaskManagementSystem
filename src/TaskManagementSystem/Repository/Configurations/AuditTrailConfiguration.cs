using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;


public class AuditTrailConfiguration : IEntityTypeConfiguration<AuditTrail>
{
    public void Configure(EntityTypeBuilder<AuditTrail> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.EntityId);
        builder.HasIndex(x => x.EntityName);
        builder.HasIndex(x => x.ParticipantName);
        builder.HasIndex(x => x.ParticipandIdentification);

        builder.Property(x => x.PerformedAction)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.EntityId)
            .IsRequired();

        builder.Property(x => x.EntityName)
            .IsRequired();

        builder.Property(x => x.ParticipantName)
            .IsRequired();

        builder.Property (x => x.ParticipandIdentification)
            .IsRequired();

        builder.Property(x => x.PerformedAt)
            .IsRequired();
    }
}
