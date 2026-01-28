using Shared.ApiResponse;
using Shared.DataTransferObjects.AnalyticsReporting.UserDashboard;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.Authentication;

public class GetUserDashboardHandler
{
	private HttpClient _httpClient;

    public GetUserDashboardHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<UserTaskDashboardDto?> Handle(int UserId)
    {
		try
		{
			var getDashboardResponse = await _httpClient.GetAsync($"api/AnalyticsReporting/userdashboard/userid/{UserId}");

			string responseContent = await getDashboardResponse.Content.ReadAsStringAsync();

			GenericResponse<UserTaskDashboardDto?>? responseData = JsonSerializer.Deserialize<GenericResponse<UserTaskDashboardDto?>>(responseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

			if(responseData is null)
			{
				return new UserTaskDashboardDto();
			}

			if(!responseData.IsSuccessful)
			{
				return new UserTaskDashboardDto();
			}

			return responseData.Data;
		}
		catch (Exception ex)
		{
			return null;
		}
    }
}
