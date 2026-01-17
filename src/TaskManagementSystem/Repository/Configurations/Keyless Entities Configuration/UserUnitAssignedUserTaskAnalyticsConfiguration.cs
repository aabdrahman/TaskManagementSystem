using Entities.Models.Keyless_Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations.Keyless_Entities_Configuration;

public class UserUnitAssignedUserTaskAnalyticsConfiguration : IEntityTypeConfiguration<UserUnitAssignedUserTaskAnalytics>
{
    public void Configure(EntityTypeBuilder<UserUnitAssignedUserTaskAnalytics> builder)
    {
        builder.HasNoKey();

        builder.ToView(null);
    }
}
