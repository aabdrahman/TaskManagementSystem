namespace TaskManagementSystem.Client.Helper;

public static class ClientHelper
{
    public static string BaseUri = "https://localhost:44365";
    public static string SecureClientKey = "SecureClient";
    public static string OpenClientKey = "OpenClient";

    public static string TokenSessionStorgaeKey = "Session-Token";

    public static string AuthorizationHeaderKey = "Bearer";

    public static int GetRefreshTokenWindow = 15;
}
