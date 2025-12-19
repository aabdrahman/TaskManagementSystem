using Shared.ApiResponse;
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

    public async Task<Shared.DataTransferObjects.UserDashboard.UserTaskDashboardDto?> Handle(int UserId)
    {
		try
		{
			var getDashboardResponse = await _httpClient.GetAsync($"api/TaskUsers/userdashboard/userid/{UserId}");

			string responseContent = await getDashboardResponse.Content.ReadAsStringAsync();

			GenericResponse<Shared.DataTransferObjects.UserDashboard.UserTaskDashboardDto?> responseData = JsonSerializer.Deserialize<GenericResponse<Shared.DataTransferObjects.UserDashboard.UserTaskDashboardDto?>>(responseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

			if(responseData is null)
			{
				return null;
			}

			if(!responseData.IsSuccessful)
			{
				return null;
			}

			return responseData.Data;
		}
		catch (Exception ex)
		{
			return null;
		}
    }
}
