namespace Shared.RequestParameters.Analytics;

public record class UserUnitTaskUserAnalyticsRequestParameter
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int? UserId { get; set; }
    public int? UnitId { get; set; }
}
