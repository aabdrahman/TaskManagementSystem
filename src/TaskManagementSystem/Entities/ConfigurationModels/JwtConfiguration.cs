namespace Entities.ConfigurationModels;

public class JwtConfiguration
{
    public string validIssuer { get; set; }
    public string[] validAudience { get; set; }
    public int expireAfter { get; set; }
    public int sessionTimeoutAfterInMinutes { get; set; }
}
